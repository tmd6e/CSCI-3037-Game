using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {
        if (aICharacter.isDead.Value)
        {
            return SwitchState(aICharacter, aICharacter.dead);
        }

        if (aICharacter.characterCombatManager.currentTarget != null && aICharacter.characterCombatManager.currentTarget.isDead.Value)
        {
            aICharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aICharacter);
            return this;
        }

        if (aICharacter.aiCharacterNetworkManager.currentToughness.Value <= 0 && aICharacter.aiCharacterNetworkManager.canBeBroken.Value)
        {
            return SwitchState(aICharacter, aICharacter.toughnessBrokenState);
        }

        

        if (aICharacter.characterCombatManager.currentTarget != null && !aICharacter.characterCombatManager.currentTarget.isDead.Value)
        {
            return SwitchState(aICharacter, aICharacter.pursueTarget);
        }
        else
        {
            // RETURN THIS STATE, TO CONTINUALLY SEARCH FOR A TARGET
            Debug.Log("SEARCHING FOR A TARGET");

            aICharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aICharacter);

            return this;
        }

    }

}
