using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Assignables
    public Transform playerCam;
    public Transform orientation;

    // Other
    private Rigidbody rb;

    // Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    // Movement
    public float moveSpeed = 500f;
    public float maxSpeed = 20f;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    // Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 150f;

    // Input
    float x, y;
    bool jumping;

    // Ground
    private Vector3 normalVector = Vector3.up;
    private bool cancellingGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MyInput();
        Look();
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
    }

    private void Movement()
    {
        // Extra gravity
        rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);

        // Find velocity relative to look direction
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        // Counter movement
        CounterMovement(x, y, mag);

        // Jump
        if (readyToJump && jumping) Jump();

        // Limit max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        // Movement multipliers (air control 0.2%)
        float multiplier = grounded ? 1f : 0.2f;

        // Apply movement forces
        rb.AddForce(orientation.forward * y * moveSpeed * multiplier, ForceMode.Acceleration);
        rb.AddForce(orientation.right * x * moveSpeed * multiplier, ForceMode.Acceleration);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            // Add jump forces
            rb.AddForce(Vector3.up * jumpForce * 1.5f, ForceMode.Impulse);
            rb.AddForce(normalVector * jumpForce * 0.5f, ForceMode.Impulse);

            // Reset vertical velocity if needed
            Vector3 vel = rb.linearVelocity;
            if (rb.linearVelocity.y < 0.5f)
                rb.linearVelocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.linearVelocity.y > 0)
                rb.linearVelocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * sensMultiplier;

        // Vertical rotation (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (player body)
        orientation.Rotate(Vector3.up * mouseX);
    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded) return;

        // Counter movement
        if ((Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f) || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
            rb.AddForce(orientation.right * -mag.x * moveSpeed * counterMovement, ForceMode.Acceleration);

        if ((Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f) || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
            rb.AddForce(orientation.forward * -mag.y * moveSpeed * counterMovement, ForceMode.Acceleration);

        // Limit diagonal speed
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 n = flatVel.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(n.x, rb.linearVelocity.y, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.linearVelocity.x, rb.linearVelocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.linearVelocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private void OnCollisionStay(Collision other)
    {
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), 0.1f);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
}