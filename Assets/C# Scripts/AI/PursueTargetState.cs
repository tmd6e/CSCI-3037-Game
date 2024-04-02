using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Pursue Target")]
public class PursueTargetState : AIState
{

    public override AIState Tick(AICharacterManager aICharacter)
    {
        

        

        // CHECK IF WE ARE PERFORMING AN ACTION (DO NOTHING UNTIL ACTION IS COMPLETE)
        if (aICharacter.isPerformingAction)
        {
            return this;
        }
        // CHECK IF OUR TARGET IS NULL, IF SO RETRUN TO IDLE
        if (aICharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return this;
        }
        // MAKE SURE OUR NAVMESH AGENT IS ACTIVE, IF ITS NOT ENABLE IT
        if (!aICharacter.navMeshAgent.enabled)
        {
            aICharacter.navMeshAgent.enabled = true;
        }

        aICharacter.aiCharacterLocomotionManager.RotateTowardAgent(aICharacter);
        // IF WE ARE WITHIN COMBAT RANGE OF A TARGET, SWITCH STATE TO COMBAT STANCE STATE

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
