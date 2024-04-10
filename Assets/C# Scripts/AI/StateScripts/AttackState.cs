using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Attack")]
public class AttackState : AIState
{
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isDead.Value) {
            return SwitchState(aiCharacter, aiCharacter.dead);
        }
        if (aiCharacter.aiCharacterNetworkManager.currentToughness.Value <= 0 && aiCharacter.aiCharacterNetworkManager.canBeBroken.Value)
        {
            return SwitchState(aiCharacter, aiCharacter.toughnessBrokenState);
        }
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null) {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }
        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value) {
            aiCharacter.aiCharacterCombatManager.currentTarget = null;
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        // Rotate towards target while attacking
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);

        // Set movement values to zero
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

        // Do a combo
        if (willPerformCombo && !hasPerformedCombo) {
            if (currentAttack.comboAction != null)
            {
                // If can combo
                //hasPerformedCombo = true;
                //currentAttack.comboAction.AttemptTOPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction)
        {
            return this;
        }

        // If recovering from an action, wait until performing another
        if (!hasPerformedAttack) {
            if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0) {
                return this;
            }
            

            PerformAttack(aiCharacter);

            return this;
        }

        //if (pivotAfterAttack) {
        //    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        //}

        return SwitchState(aiCharacter, aiCharacter.combatStance);
    }

    protected void PerformAttack(AICharacterManager aiCharacter) {
        hasPerformedAttack = true;
        currentAttack.AttemptTOPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);

        hasPerformedAttack = false;
        hasPerformedCombo = false;
    }
}
