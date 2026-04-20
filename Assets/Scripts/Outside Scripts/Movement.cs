using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Rigidbody rb;
    public GameObject CamHolder;
    public float speed = 5f;
    public float runSpeed = 9f;
    public float maxForce = 10f;
    public float jumpForce = 3f;
    public float airControl = 1f;

    [Header("Look Settings")]
    public float sensitivity = 100f;
    private Vector2 move, look;
    private float lookRotation;

    [Header("Grounded")]
    public bool grounded;

    [Header("Stamina Settings")]
    public StaminaBar staminaBar;
    public float maxStamina = 100f;
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    private float currentStamina;
    private bool isRunning;
    

    // Input Callbacks
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        //Debug.Log(move);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            Jump();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed) isRunning = true;
        else if (context.canceled) isRunning = false;
    }

    // Unity Methods
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;

        currentStamina = maxStamina;
        if (staminaBar != null)
            staminaBar.SetMaxStamina(maxStamina);
    }

    void Update()
    {
        Look();
        HandleStamina();
    }

    void FixedUpdate()
    {
        Move();
    }

    // Movement Methods
    void Move()
    {
        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // Determine speed (running if possible)
            float currentSpeed;

            if (isRunning && grounded && currentStamina > 0 && move.y > 0)
        {
            // If the player is holding run, is on the ground, has stamina, and pressing forward
            currentSpeed = runSpeed;
        }
            else
        {
            // Otherwise, normal walking speed
            currentSpeed = speed;
        }
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y) * currentSpeed;
        targetVelocity = transform.TransformDirection(targetVelocity);

        Vector3 velocityChange = targetVelocity - currentVelocity;
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);

       // Air Control Restriction
        if (!grounded)
    {
        // Only allow forward movement in air
        if (move.y > 0) // W pressed
    {
            // Apply air control normally
            velocityChange.x *= 0f; // block sideways
            velocityChange.z *= airControl; // allow forward
    }
        else
    {
            // Not moving forward → block all air movement
            velocityChange.x = 0f;
            velocityChange.z = 0f;
    }
}

        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    void Jump()
    {
        if (grounded)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    void Look()
    {
        // Turn
        transform.Rotate(Vector3.up * look.x * sensitivity * Time.deltaTime);

        // Pitch
        lookRotation += (-look.y * sensitivity * Time.deltaTime);
        lookRotation = Mathf.Clamp(lookRotation, -90f, 90f);
        CamHolder.transform.localRotation = Quaternion.Euler(lookRotation, 0f, 0f);
    }

    // Stamina Handling
    void HandleStamina()
    {
        if (isRunning && grounded && move.y > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isRunning = false; // stop sprinting when out of stamina
            }
        }
        else
        {
            // Regenerate stamina when not running
            currentStamina += staminaRegenRate * Time.deltaTime;
            if (currentStamina > maxStamina)
                currentStamina = maxStamina;
        }

        if (staminaBar != null)
            staminaBar.SetStamina(currentStamina);
    }

    // Grounded Setter
    public void SetGrounded(bool state)
    {
        grounded = state;
    }
}