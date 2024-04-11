using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
public class PursueTargetState : AIState
{

    public override AIState Tick(AICharacterManager aICharacter)
    {

        if (aICharacter.isDead.Value)
        {
            return SwitchState(aICharacter, aICharacter.dead);
        }
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
        // OPTION 01
        //if (aICharacter.aiCharacterCombatManager.distanceFromTarget <= aICharacter.combatStance.maximumEngagementDistance) {
        //    return SwitchState(aICharacter,aICharacter.combatStance);
        //}
        // OPTION 02
        if (aICharacter.aiCharacterCombatManager.distanceFromTarget <= aICharacter.navMeshAgent.stoppingDistance) { 
            return SwitchState(aICharacter, aICharacter.combatStance);
        }
        // IF THE TARGET IS NOT REACHABLE, AND THEY ARE FAR AWAY, RETURN HOME

        // PURSUE THE TARGET
        // OPTION 01
        //aICharacter.navMeshAgent.SetDestination(aICharacter.aiCharacterCombatManager.currentTarget.transform.position);

        // OPTION 02
        NavMeshPath path = new NavMeshPath();
        aICharacter.navMeshAgent.CalculatePath(aICharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aICharacter.navMeshAgent.SetPath(path);

        return this;
    }
}
