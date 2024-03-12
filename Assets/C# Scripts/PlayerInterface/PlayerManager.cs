using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    
    protected override void Awake()
    {
        base.Awake();

        // Initialize player

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
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

        // Regenerate stamina
        playerStatsManager.RegenerateStamina();
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

            // Update max health and stamina when powerup changes multipliers
            playerNetworkManager.healthMultiplier.OnValueChanged += playerNetworkManager.SetNewHealthValue;
            playerNetworkManager.staminaMultiplier.OnValueChanged += playerNetworkManager.SetNewStaminaValue;

            // Updates stats when stats change
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHUDManager.SetNewHealth;
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHUDManager.SetNewStamina;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealth(playerNetworkManager.healthMultiplier.Value);
            playerNetworkManager.currentHealth.Value = playerStatsManager.CalculateHealth(playerNetworkManager.healthMultiplier.Value);
            PlayerUIManager.instance.playerUIHUDManager.SetMaxHealth(playerNetworkManager.maxHealth.Value);

            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStamina(playerNetworkManager.staminaMultiplier.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStamina(playerNetworkManager.staminaMultiplier.Value);
            PlayerUIManager.instance.playerUIHUDManager.SetMaxStamina(playerNetworkManager.maxStamina.Value);
        }
    }
}
