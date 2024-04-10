using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBossCharacterManager : AICharacterManager
{
    public int bossID = 0;
    [SerializeField] bool hasBeenDefeated = false;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {

        }
        else {

            if (hasBeenDefeated) {
                aiCharacterNetworkManager.isActive.Value = false;
            }
        }
    }
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
            canMove = false;
            canRotate = false;

            // Reset flags

            // If not grounded, play aerial death

            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Die", true);
            }

            hasBeenDefeated = true;
        }

        // Play SFX
        yield return new WaitForSeconds(5);
        // Award players with currency that can buy powerups

    }



}
