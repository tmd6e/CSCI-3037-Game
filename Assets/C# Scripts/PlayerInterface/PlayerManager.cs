using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    protected override void Awake()
    {
        base.Awake();

        // Initialize player

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
    }
    protected override void Update()
    {
        base.Update();

        // Disables unwanted behavior from non-owners of this network instance
        if (!IsOwner) {
            return;
        }

        // Handles player movement
        playerLocomotionManager.HandleAllMovement();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner) {
            return;
        }

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // If this is the local client, assign the camera
        if (IsOwner) {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
        }
    }
}
