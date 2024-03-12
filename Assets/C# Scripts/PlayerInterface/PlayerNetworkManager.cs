using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void SetNewHealthValue(float oldMultiplier, float newMultiplier) {
        maxHealth.Value = player.playerStatsManager.CalculateHealth(newMultiplier);
    }
    public void SetNewStaminaValue(float oldMultiplier, float newMultiplier)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStamina(newMultiplier);
    }
}
