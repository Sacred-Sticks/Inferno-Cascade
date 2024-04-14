using Kickstarter.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour, IInputReceiver
{
    [Header("Inputs")]
    [SerializeField] private Vector2Input movementInput;
    [SerializeField] private FloatInput jumpInput;

    [Header("Movement")]
    public float moveSpeed;

    public float groundrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    #region InputHandler
    public void RegisterInputs(Player.PlayerIdentifier playerIdentifier)
    {
        movementInput.RegisterInput(OnMovementInputChange, playerIdentifier);
        jumpInput.RegisterInput(OnJumpInputChange, playerIdentifier);
    }

    public void DeregisterInputs(Player.PlayerIdentifier playerIdentifier) 
    {
        movementInput.DeregisterInput(OnMovementInputChange, playerIdentifier);
        jumpInput.DeregisterInput(OnJumpInputChange, playerIdentifier);
    }

    private void OnMovementInputChange(Vector2 input) 
    {
        horizontalInput = input.x;
        verticalInput = input.y;
    }

    private void OnJumpInputChange(float input)
    {
        if (input == 0)
            return;

        readyToJump = false;
        Jump();
        Invoke(nameof(ResetJump), jumpCooldown);
    }
    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        SpeedControl();

        //handle drag
        if (grounded)
            rb.drag = groundrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    //i think we dont need but will add anyway just in case
    private void SpeedControl() 
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if(flatVel.magnitude > moveSpeed) 
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump() 
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    public void ResetJump() 
    {
        readyToJump = true;
    }
}

