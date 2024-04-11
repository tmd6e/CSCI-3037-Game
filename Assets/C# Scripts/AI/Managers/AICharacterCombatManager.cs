using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    [Header("Boss/Elite Enemy Exclusive")]
    public bool phase2Triggered = false;

    [Header("Action Recovery Timer")]
    public float actionRecoveryTimer = 0;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius_ = 15;
    [SerializeField] float minimumDetectionAngle = -35;
    [SerializeField] float maximumDetectionAngle = 35;

    [Header("Attack Rotation")]
    public float attackRotationSpeed = 25;

    public void FindATargetViaLineOfSight(AICharacterManager aICharacter)
    {
        if (currentTarget != null) return;
        Collider[] colliders = Physics.OverlapSphere(aICharacter.transform.position,
            detectionRadius_, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();
            if (targetCharacter == null) continue;
            if (targetCharacter == aICharacter) continue;
            if (targetCharacter.isDead.Value) continue;

            // Can we attack this character?
            if (WorldUtilityManager.instance.CanIDamageThisTarget(aICharacter.characterGroup, targetCharacter.characterGroup))
            {
                // If a target is found, it needs to be infront of us
                Vector3 targetDirection = targetCharacter.transform.position - aICharacter.transform.position;
                float viewableAngle = Vector3.Angle(targetDirection, aICharacter.transform.forward);

                if (viewableAngle > minimumDetectionAngle && viewableAngle < maximumDetectionAngle)
                {
                    // Check for environment blocks
                    if (Physics.Linecast(aICharacter.characterCombatManager.lockOnTransform.position, 
                        targetCharacter.characterCombatManager.lockOnTransform.position,
                        WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        Debug.DrawLine(aICharacter.characterCombatManager.lockOnTransform.position,
                            targetCharacter.characterCombatManager.lockOnTransform.position);
                        Debug.Log("BLOCKED");
                    }
                    else
                    {
                        aICharacter.characterCombatManager.SetTarget(targetCharacter);
                    }

                }
            }
        }

    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter) {
        if (aiCharacter.aiCharacterNetworkManager.isMoving.Value) {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter)
    {
        if (currentTarget == null)
        {
            return;
        }
        // Check if rotation is available
        if (!aiCharacter.canRotate)
        {
            return;
        }

        if (!aiCharacter.isPerformingAction) {
            return;
        }

        // Rotate towards target
        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero)
        {
            targetDirection = aiCharacter.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
    }
    public void HandleActionRecovery(AICharacterManager aiCharacter) {
        if (actionRecoveryTimer > 0) {
            if (aiCharacter.isPerformingAction) {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }
}
