using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{

    [Header("Detection")]
    [SerializeField] float detectionRadius_ = 15;
    [SerializeField] float minimumDetectionAngle = -35;
    [SerializeField] float maximumDetectionAngle = 35;

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
}
