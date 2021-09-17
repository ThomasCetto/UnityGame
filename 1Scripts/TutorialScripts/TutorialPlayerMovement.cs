using UnityEngine;

public class TutorialPlayerMovement : MonoBehaviour
{
    private float playerHeight = 2f;

    [SerializeField] Transform orientation;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float airMultiplier = 0.4f;
    float movementMultiplier = 10f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 21f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded;

    [Header("Crouching")]
    Vector3 crouchScale = new Vector3(1f, 0.7f, 1f); //rende l'altezza 7/10 rispetto all'originale
    private bool isCrouching = false;
    [SerializeField] private float crouchSpeed = 4f;




    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {

        MovePlayer();
    }

    private void Update()
    {

        MovementMechanics();

    }

    private void MovementMechanics()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();

        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
            Jump();

        if (Input.GetKeyDown(crouchKey))
            Crouch();

        if (Input.GetKeyUp(crouchKey))
            StopCrouching();


        //rb.useGravity = !(verticalMovement == 0 && horizontalMovement == 0 && OnSlope()); //fa in modo che non si scivoli sulle pendenze
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void ControlSpeed()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
    }

    void ControlDrag()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }



    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }

        if (isCrouching)
            rb.velocity *= crouchSpeed / moveSpeed;
    }

    private bool OnSlope()
    {
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
                return true;
        }
        return false;
    }

    private void Crouch()
    {
        isCrouching = true;
        transform.localScale = crouchScale;
        playerHeight *= 0.7f;

        //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), Time.deltaTime * 1f);

        //transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1f, 0.7f, 1f), Time.deltaTime * 1f);

        transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);

        transform.localScale = crouchScale;
    }

    private void StopCrouching()
    {
        isCrouching = false;
        playerHeight = 2;

        //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), Time.deltaTime * 6f);
        //transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 6f);

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

        transform.localScale = Vector3.one;
    }
}
