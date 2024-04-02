using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aICharacter)
    {

        if (aICharacter.characterCombatManager.currentTarget != null)
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
