using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Sword;
    public bool CanAttack = true;
    public float AttackCooldown = 1.0f;
    public float AttackDuration = 1.0f;
    public bool IsAttacking = false;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(CanAttack)
            {
                SwordAttack();
            }
        }
    }

    public void SwordAttack()
    {
        IsAttacking = true;
        CanAttack = false;
        Debug.Log("Player is attacking");


        /* Psuedocode once sword animation is made
        Animator anim = Sword.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        */
        StartCoroutine(AttackRoutine());
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
