using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Combat Stance")]
public class CombatStanceState : AIState
{
    // Select an attack
    // Process combat logic while awaiting attack
    // Switch to pursuit if out of range
    // If target is not present, switch to idle

    [Header("Attacks")]
    public List<AICharacterAttackAction> aICharacterAttacks; // All attacks in moveset
    protected List<AICharacterAttackAction> potentialAttacks; // All possible attacks
    private AICharacterAttackAction chosenAttack;
    private AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToPerformCombo = 25;
    [SerializeField] protected bool hasRolledForComboChance = false;

    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 1;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isDead.Value)
        {
            return SwitchState(aiCharacter, aiCharacter.dead);
        }
        if (aiCharacter.aiCharacterNetworkManager.currentToughness.Value <= 0 && aiCharacter.aiCharacterNetworkManager.canBeBroken.Value)
        {
            return SwitchState(aiCharacter, aiCharacter.toughnessBrokenState);
        }
        if (aiCharacter.isPerformingAction) {
            return this;
        }
        if (!aiCharacter.navMeshAgent.enabled) {
            aiCharacter.navMeshAgent.enabled = true;
        }

        // Turn toward target
        //if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value) {
        //    if (aiCharacter.aiCharacterCombatManager.viewableAngle <= 30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30) {
        //        aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        //    }
        //}

        // Rotate to face target
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        // Idle if there is no target
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null) {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead.Value)
        {
            aiCharacter.aiCharacterCombatManager.currentTarget = null;
            return SwitchState(aiCharacter, aiCharacter.idle);
        }
        

        // Get an attack if there is not one
        if (!hasAttack)
        {
            GetNewAttack(aiCharacter);
        }
        else {
            // Pass attack to attack state
            aiCharacter.attack.currentAttack = chosenAttack;
            // Roll for combo
            // Switch state
            return SwitchState(aiCharacter, aiCharacter.attack);
        }

        // Chase if out of engagement distance
        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance) {
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        }

        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter) { 
        potentialAttacks = new List<AICharacterAttackAction>();

        // Sort through possible attacks
        foreach (var potentialAttack in aICharacterAttacks) {
            // Verify attack range
            if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget) {
                continue;
            }
            if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
            {
                continue;
            }

            // Verify attack angle
            //if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle) {
            //    continue;
            //}
            //if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle) {
            //    continue;
            //}

            // Place remaining attack back in list
            potentialAttacks.Add(potentialAttack);
        }

        if (potentialAttacks.Count <= 0) {
            return;
        }

        var totalWeight = 0;

        // Choose random attack based on weight
        foreach (var attack in potentialAttacks) {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks) { 
            processedWeight += attack.attackWeight;
            // Select the attack and pass to attack state
            if (randomWeightValue <= processedWeight) {
                chosenAttack = attack;
                previousAttack = chosenAttack;
                hasAttack = true;
                return;
            }
        }
        
        
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance) {
        bool outcomeWillBePerformed = false;

        int randomPercent = Random.Range(0, 100);

        if (randomPercent < outcomeChance) {
            outcomeWillBePerformed = true;
        }
        return outcomeWillBePerformed;
    }

    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);

        hasAttack = false;
        hasRolledForComboChance = false;  
    }
}
