using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    PlayerInputManager playerInputManager;
    // Values retrieved from input manager
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float movementAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float rotationSpeed = 5;
    [SerializeField] float sprintingSpeed = 8;
    [SerializeField] float sprintingStaminaCost = 2.0f;

    [Header("Jump")]
    [SerializeField] float jumpStaminaCost = 25;
    [SerializeField] float jumpHeight = 4;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float freeFallSpeed = 2;
    private Vector3 jumpDirection;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 25;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update() {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.vertical.Value = verticalMovement;
            player.characterNetworkManager.horizontal.Value = horizontalMovement;
            player.characterNetworkManager.networkMoveAmount.Value = movementAmount;
        }
        else {
            verticalMovement = player.characterNetworkManager.vertical.Value;
            horizontalMovement = player.characterNetworkManager.horizontal.Value;
            movementAmount = player.characterNetworkManager.vertical.Value;

            //If not locked on
            if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, movementAmount, player.playerNetworkManager.isSprinting.Value);
            }
            //If locked on
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalMovement, verticalMovement, player.playerNetworkManager.isSprinting.Value);
            }
        }
    }

    public void HandleAllMovement() {
        //Grounded
        HandleGroundedMovement();
        //Aerial
        HandleRotation();
        HandleJumpingMovement();
    }

    private void GetVerticalHorizontalInput() {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        movementAmount = PlayerInputManager.instance.movementAmount;
        // Clamp movement
    }

    private void HandleGroundedMovement()
    {
        if (player.isDead.Value)
        {
            return;
        }
        if (!player.canMove)
        {
            return;
        }

        GetVerticalHorizontalInput();
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (PlayerInputManager.instance.movementAmount > 0.5f)
            {
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.movementAmount <= 0.5f)
            {
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if (player.isDead.Value) {
            return;
        }
        if (!player.canRotate){
            return;
        }
        if (player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerNetworkManager.isSprinting.Value || player.playerLocomotionManager.isRolling)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                {
                    targetDirection = transform.forward;
                }

                Quaternion targetLockOnRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetLockOnRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else
            {
                if (player.playerCombatManager.currentTarget == null)
                {
                    return;
                }

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Quaternion targetLockOnRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetLockOnRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
        }
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            targetRotationDirection.Normalize();
            targetRotationDirection.y = 0;


            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

    public void Sprint() {
        if (player.isPerformingAction) {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        // If stamina is depleted, exit sprint
        if (player.playerNetworkManager.currentStamina.Value <= 0) {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }
        

        // If moving, sprint
        if (movementAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        // If stationary, don't sprint
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value) {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void DodgeAttempt() {
        if (player.isPerformingAction || player.isDead.Value)
        {
            return;
        }
        // Check if sufficient stamina
        if (player.playerNetworkManager.currentStamina.Value <= 0) {
            return;
        }

        // If moving while trying to dodge, roll
        if (movementAmount > 0)
        {
            //Roll
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;
            player.playerAnimatorManager.PlayTargetActionAnimation("RollForward", true, true, false, false);
            player.playerLocomotionManager.isRolling = true;
        }
        // If still, perform a backstep
        else {
            //Backstep
            player.playerAnimatorManager.PlayTargetActionAnimation("Backstep", true, true, false, false);
        }

        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }

    public void ApplyJumpingVelocity()
    {
        // Apply an upward velocity depending on forces
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    public void JumpAttempt()
    {
        Debug.Log("Attempting jump");
        // If we are perfroming a general action, we do not want to allow a jump (will change when combat is added)
        if (player.isPerformingAction)
        {
            return;
        }
        // Check if sufficient stamina
        // If we are out of stamina, we do not wish to allow a jump
        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        // If we are already in a jump, we do not want to allow a jump again until the current jump has finished
        if (player.isJumping)
        {
            return;
        }

        // If we are not grounded, we do not want to allow a jump
        if (!player.isGrounded)
        {
            return;
        }

        
        player.playerAnimatorManager.PlayTargetActionAnimation("Jump", false);

        player.isJumping = true;

        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

        jumpDirection.y = 0;

        //ApplyJumpingVelocity();

        if (jumpDirection != Vector3.zero)
        {



            // If we are sprinting, jump direction is at full distance
            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1;
            }

            // If we are running, jump direction is at half distance
            else if (PlayerInputManager.instance.movementAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }

            // If we are walking, jump direction is at quarter distance
            else if (PlayerInputManager.instance.movementAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }
        }
        else
        {

        }
    }
}
