using Kickstarter.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inferno_Cascade
{
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
        public LayerMask whatIsGround;
        bool grounded;

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

            Registry.Register(RegistryStrings.PlayerRigidbody, rb);
        }

        private void Update()
        {
            //ground check
            float range = 1.1f;
            grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, range, whatIsGround);

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
            moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

            // rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            if (rb.velocity.sqrMagnitude > moveSpeed * moveSpeed)
                return;

            if (horizontalInput == 0 && verticalInput == 0)
            {
                if(grounded)
                    moveDirection = -rb.velocity.normalized;
                if (rb.velocity.sqrMagnitude < 0.25f)
                    moveDirection = Vector3.zero;
            }

            //on ground
            if (grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

            //in air
            else if (!grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        }

        private void Jump()
        {
            // reset y velocity
            if (!grounded) { return; }
            //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce - rb.velocity.y * Vector3.up, ForceMode.VelocityChange);
        }

        public void ResetJump()
        {
            readyToJump = true;
        }
    }

}