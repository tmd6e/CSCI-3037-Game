using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private CharacterManager character;
    [SerializeField] private Animator anim;
    public float attackStaminaCost = 10;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    public float AttackDuration = 1.0f;
    public bool IsAttacking = false;

    private void Awake()
    {
        // Retrieve character animator
        anim = GetComponentInParent<Animator>();
        character = GetComponentInParent<CharacterManager>();

    }
    void Update()
    {
    }

    public void SwordAttack()
    {
        // Change attack speed based on buffs
        character.animator.SetFloat("AttackSpeed", character.characterNetworkManager.attackSpeed.Value);
        // Call the routine if stamina is sufficient
        if (character.characterNetworkManager.currentStamina.Value >= attackStaminaCost)
        {
            IsAttacking = true;
            CanAttack = false;
            Debug.Log("Player is attacking");

            anim.SetTrigger("Attack");
            StartCoroutine(AttackRoutine());
        }
    }

    // Must reset bool variable CanAttack to true
    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(AttackDuration); // Wait for the attack duration
        IsAttacking = false; // After the attack, the player is no longer attacking

        yield return new WaitForSeconds(AttackCooldown - AttackDuration); // Wait for the remainder of the cooldown
        CanAttack = true; // Player can attack again after the cooldown
    }
}
