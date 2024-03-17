using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
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
    }

    private void GetVerticalHorizontalInput() {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        movementAmount = PlayerInputManager.instance.movementAmount;
        // Clamp movement
    }

    private void HandleGroundedMovement()
    {
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
        if (player.isPerformingAction)
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
}