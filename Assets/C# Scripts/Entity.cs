using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int health = 100;
    public int attackPower = 50;
    protected Collider collider;
    public GameObject referencedGameObject;

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
    public virtual void TakeDamage(Entity attacker)
    {
        // Damage will be a placeholder until we figure out how to receive the attacker's attack power for proper damage receival
        health -= attacker.attackPower;
    }
    private void Start()
    {
        collider = GetComponent<Collider>();
    }
    private void Update()
    {
        if (health <= 0) {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.enabled = false;
        }
    }    
}
