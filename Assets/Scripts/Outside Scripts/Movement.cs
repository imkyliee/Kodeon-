using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
     public Rigidbody rb;
     public GameObject CamHolder;
     public float speed, sensitivity, maxForce, jumpForce, airControl;
     private Vector2 move, look;
     private float lookRotation;
     public bool grounded;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        //Debug.Log("move.x: " + move.x + ", move.y: " + move.y);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
         if (context.performed)
    {
        Jump();
    }
    }

    private void FixedUpdate()
    {
      Move();
    }

    void Jump()
        {
           Vector3 jumpForces = Vector3.zero;
              if (grounded)
              {
                  rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
              }
        }
    void Move()
        {
    Vector3 currentVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

    Vector3 targetVelocity = new Vector3(move.x, 0, move.y) * speed;
    targetVelocity = transform.TransformDirection(targetVelocity);

    Vector3 velocityChange = targetVelocity - currentVelocity;
    velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

        //Air Control
     if (!grounded)
        {
            // Apply air control to X-axis (A/D movement)
            velocityChange.x *= airControl;
            // Apply air control to Z-axis (W/S movement)
            velocityChange.z *= airControl;

        }

    rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
void Look()
    {
        //Turn
        transform.Rotate(Vector3.up * look.x * sensitivity * Time.deltaTime);

        //Look
        lookRotation += (-look.y * sensitivity * Time.deltaTime);
        lookRotation = Mathf.Clamp(lookRotation, -90f, 90f);
        CamHolder.transform.localRotation = Quaternion.Euler(lookRotation, 0f, 0f);
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       Cursor.lockState = CursorLockMode.Locked; // Lock cursor
    }

    void Update()
    {
        Look();
    }
       public void SetGrounded(bool state)
    {
        grounded = state;
    }
    
}