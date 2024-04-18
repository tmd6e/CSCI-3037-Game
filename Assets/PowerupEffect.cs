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
            player.playerNetworkManager.attackMultiplier.Value += attackBoost;
            player.playerNetworkManager.breakMultiplier.Value += breakBoost;
            player.playerNetworkManager.healthMultiplier.Value += healthBoost;
            player.playerNetworkManager.staminaMultiplier.Value += staminaBoost;
            player.playerNetworkManager.attackSpeed.Value += attackSpeedBoost;
            player.playerLocomotionManager.walkingSpeed += speedBoost;
            player.playerLocomotionManager.runningSpeed += speedBoost;
            player.playerLocomotionManager.sprintingSpeed += speedBoost;
            powerupProcced = true;
            Destroy(gameObject);
        }
    }
}
