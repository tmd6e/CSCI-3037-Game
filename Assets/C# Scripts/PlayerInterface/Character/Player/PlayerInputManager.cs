using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public PlayerManager player;
    PlayerControls playerControls;

    [Header("Movement")]
    [SerializeField] Vector2 movementAxis;

    public float horizontalInput;
    public float verticalInput;
    public float movementAmount;

    [Header("Actions")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool attackInput = false;

    [Header("Camera")]
    [SerializeField] Vector2 cameraAxis;
    public float camHorizontalInput;
    public float camVerticalInput;
    [SerializeField] bool lockOnInput;
    [SerializeField] bool lockOnLeftInput;
    [SerializeField] bool lockOnRightInput;
    private Coroutine lockOnCoroutine;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Debug.Log("Dupe detected");
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        // Remember to set back to false once the menu is fixed
        instance.enabled = false;
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene) {
        // If the player is loading into the scene, enable the player's controls

        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        // Otherwise, disable the controls (Main Menu UI instance)
        // Prevents player from moving while menu navigates
        else {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null) {
            playerControls = new PlayerControls();

            // Movement actions
            playerControls.PlayerMovement.Movement.performed += i => movementAxis = i.ReadValue<Vector2>(); //Retrieve movement data from 'joystick' and feed into movement axis
            playerControls.PlayerCamera.CameraControls.performed += i => cameraAxis = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            // Lock on action
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOnLeftInput = true;
            playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOnRightInput = true;

            // Sprint action
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            // Attack action
            playerControls.PlayerActions.Attack.performed += i => attackInput = true;
        }

        playerControls.Enable();
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnApplicationFocus(bool focus)
    {
        // Disables inputs on minimization
        if (enabled) {
            if (focus)
            {
                playerControls.Enable();
            }
            else {
                playerControls.Disable();
            }
        }
    }
    private void Update()
    {
        if (player == null) {
            return;
        }
        HandleAllInputs();
    }
    private void HandleAllInputs() {
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandleMovementInput();
        HandleCameraMovement();
        HandleDodgeInput();
        HandleSprinting();
        HandleJumpInput();
        HandleAttackInput();
    }

    private void HandleAttackInput() {
        if (attackInput && !player.isDead.Value)
        {
            attackInput = false;
            player.weaponController.SwordAttack();
        }
    }

    private void HandleLockOnInput() {
        // Target dead?
        if (player.playerNetworkManager.isLockedOn.Value) {
            if (player.playerCombatManager.currentTarget == null)
            {
                return;
            }
            if (player.playerCombatManager.currentTarget.isDead.Value) {
                player.playerNetworkManager.isLockedOn.Value = false;
            }

            // Attempt to find a new target
            if (lockOnCoroutine != null) { 
                StopCoroutine(lockOnCoroutine);
            }
            lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
        }
        // Are we already locked on or dead?
        if (lockOnInput && player.playerNetworkManager.isLockedOn.Value || player.isDead.Value) {
            lockOnInput = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;
            // Disable lock on
            return;
        }
        if (lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;
            // Enable lock on
            PlayerCamera.instance.HandleLockOn();

            if (PlayerCamera.instance.nearestLockOnTarget != null) {
                // Set the target as current target
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }

    private void HandleLockOnSwitchTargetInput() {
        if (lockOnLeftInput) {
            lockOnLeftInput = false;

            if (player.playerNetworkManager.isLockedOn.Value) {
                PlayerCamera.instance.HandleLockOn();

                if (PlayerCamera.instance.leftLockOnTarget != null) {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        if (lockOnRightInput)
        {
            lockOnRightInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLockOn();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }

    private void HandleMovementInput() {
        verticalInput = movementAxis.y;
        horizontalInput = movementAxis.x;

        // Returns the absolute movement number
        movementAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        // Clamp values for smoother movement
        if (movementAmount <= 0.5 && movementAmount > 0)
        {
            movementAmount = 0.5f;
        }
        else if (movementAmount >= 0.5 && movementAmount <= 1)
        {
            movementAmount = 1;
        }

        // Moves forward and rotates in movement direction in regular camera
        if (player == null) {
            return;
        }

        if (movementAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, movementAmount, player.playerNetworkManager.isSprinting.Value);
        }
        else {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.playerNetworkManager.isSprinting.Value);
        }
    }

    private void HandleCameraMovement() {
        camVerticalInput = cameraAxis.y;
        camHorizontalInput = cameraAxis.x;
    }
    private void HandleDodgeInput() {
        if (dodgeInput) { 
            dodgeInput = false;
            //If UI is open, do nothing
            //Perform a dodge roll
            player.playerLocomotionManager.DodgeAttempt();
        }
    }
    private void HandleSprinting() {
        if (sprintInput)
        {
            player.playerLocomotionManager.Sprint();
        }
        else {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            // If we have a UI window open, simply return without doing anything

            // Attempt to perform jump
            player.playerLocomotionManager.JumpAttempt();
        }
    }
}
