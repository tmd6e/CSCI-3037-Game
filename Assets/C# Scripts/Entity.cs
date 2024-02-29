using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public int health = 100;
    public Hitbox[] attackHitboxes;
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
    private void Start()
    {
    }
    private void Update()
    {
        if (health <= 0) {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            mr.enabled = false;
        }
    }
}
