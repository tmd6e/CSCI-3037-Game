using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupEffect : MonoBehaviour
{
    private PlayerManager player;
    public float attackBoost;
    public float breakBoost;
    public float healthBoost;
    public float staminaBoost;
    public float speedBoost;
    public float attackSpeedBoost;
    [SerializeField] private bool powerupProcced = false;

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlayerManager>();
        if (player != null && !powerupProcced) {
            if (player.playerNetworkManager.attackMultiplier.Value + attackBoost > 0) 
            {
                player.playerNetworkManager.attackMultiplier.Value += attackBoost;
            }
            else {
                player.playerNetworkManager.attackMultiplier.Value = 0.1f;
            }

            if (player.playerNetworkManager.breakMultiplier.Value + breakBoost > 0)
            {
                player.playerNetworkManager.breakMultiplier.Value += breakBoost;
            }
            else {
                player.playerNetworkManager.breakMultiplier.Value = 0.1f;
            }
            
            if (player.playerNetworkManager.healthMultiplier.Value + healthBoost > 0)
            {
                player.playerNetworkManager.healthMultiplier.Value += healthBoost;
            }
            else {
                player.playerNetworkManager.healthMultiplier.Value = 0.05f;
            }

            if (player.playerNetworkManager.staminaMultiplier.Value + staminaBoost > 0)
            {
                player.playerNetworkManager.staminaMultiplier.Value += staminaBoost;
            }
            else
            {
                player.playerNetworkManager.staminaMultiplier.Value = 0.05f;
            }

            if (player.playerNetworkManager.attackSpeed.Value + attackSpeedBoost > 0)
            {
                player.playerNetworkManager.attackSpeed.Value += attackSpeedBoost;
            }

            if (player.playerLocomotionManager.walkingSpeed + speedBoost > 0) {
                player.playerLocomotionManager.walkingSpeed += speedBoost;
            }

            if (player.playerLocomotionManager.runningSpeed + speedBoost > 0)
            {
                player.playerLocomotionManager.runningSpeed += speedBoost;
            }

            if (player.playerLocomotionManager.sprintingSpeed + speedBoost > 0)
            {
                player.playerLocomotionManager.sprintingSpeed += speedBoost;
            }

            powerupProcced = true;
            Destroy(gameObject);
        }
    }
}
