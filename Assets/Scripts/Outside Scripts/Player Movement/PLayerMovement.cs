using UnityEngine;

public class PLayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed, groundDrag;
    [Header("Ground Checker")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    public Transform orientation;

    float horizontalInput, verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // prevent unwanted rotation from physics interactions
    }

    // Update is called once per frame
    void Update()
    {
        //ground Checker
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        //handles drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
        MyInput();
    }
    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    void Move()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }
    void FixedUpdate()
    {
        Move();
    }
}

