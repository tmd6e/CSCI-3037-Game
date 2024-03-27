using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int health = 100;
    public int toughness = 0;
    public bool toughnessMeter = false;
    public GameObject referencedGameObject;
    public GameObject objectDestructionFlag;

    /*
    // Default constructor
    public Entity()
    {
        health = 100;
        attackPower = 15;
    }
    // Constructor
    public Entity(int newHealth, int newAttackPower)
    {
        health = newHealth;
        attackPower = newAttackPower;
    }
    */
    // Actions

    public void TakeDamage(int damage, bool bypassToughness = false)
    {
        if (toughnessMeter && toughness > 0 && !bypassToughness)
        {

            toughness -= 25; // Adjust this section as needed
            health -= (int)(damage * 0.1);
        }
        else
        {
            health -= damage;
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            Debug.Log("Pass");
            //animator.SetTrigger("GotHit");
        }
    }

    void Die()
    {
        //animator.SetBool("isDead", true);
        // Can wait for the death animation to finish before destruction if you have the length
        // Destroy(this.gameObject, animationClip.length);
        Destroy(this.gameObject); // Immediate destruction
    }

    private void Start()
    {
        
    }
    private void Update()
    {
    }
}
