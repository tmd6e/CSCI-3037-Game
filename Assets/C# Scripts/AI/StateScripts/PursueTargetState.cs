using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
public class PursueTargetState : AIState
{

    public override AIState Tick(AICharacterManager aICharacter)
    {
        // SWITCH TO DEATH STATE IF DEAD
        if (aICharacter.isDead.Value)
        {
            return SwitchState(aICharacter, aICharacter.dead);
        }
        // SWITCH TO TOUGHNESS BROKEN STATE IF AI HAS TOUGHNESS AND TOUGHNESS REACHES ZERO
        if (aICharacter.aiCharacterNetworkManager.currentToughness.Value <= 0 && aICharacter.aiCharacterNetworkManager.canBeBroken.Value)
        {
            return SwitchState(aICharacter, aICharacter.toughnessBrokenState);
        }

        // CHECK IF WE ARE PERFORMING AN ACTION (DO NOTHING UNTIL ACTION IS COMPLETE)
        if (aICharacter.isPerformingAction)
        {
            return this;
        }
        // CHECK IF OUR TARGET IS NULL, IF SO RETRUN TO IDLE
        if (aICharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }
        if (aICharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
        {
            aICharacter.aiCharacterCombatManager.currentTarget = null;
            return SwitchState(aICharacter, aICharacter.idle);
        }
        // MAKE SURE OUR NAVMESH AGENT IS ACTIVE, IF ITS NOT ENABLE IT
        if (!aICharacter.navMeshAgent.enabled)
        {
            aICharacter.navMeshAgent.enabled = true;
        }

        aICharacter.aiCharacterLocomotionManager.RotateTowardAgent(aICharacter);
        // IF WE ARE WITHIN COMBAT RANGE OF A TARGET, SWITCH STATE TO COMBAT STANCE STATE
        if (aICharacter.aiCharacterCombatManager.distanceFromTarget <= aICharacter.navMeshAgent.stoppingDistance) { 
            return SwitchState(aICharacter, aICharacter.combatStance);
        }

        // FIND THE PATH
        NavMeshPath path = new NavMeshPath();
        aICharacter.navMeshAgent.CalculatePath(aICharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aICharacter.navMeshAgent.SetPath(path);

        return this;
    }
}
