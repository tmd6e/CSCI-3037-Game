using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //Unique stats to the player
    public PlayerHitbox[] attackHitboxes;
    float attackSpeed = 1.0f;
    public int toughnessBreak = 10;
    //Values offered by powerup
    
    int flatAttackBonus = 0;
    int flatHPBonus = 0;
    float attackMultiplier = 1.0f;
    float attackSpeedMultiplier = 1.0f;
    float maxHPMultiplier = 1.0f;
    float toughnessBreakMultiplier = 1.0f;
    MovementScript movementScript;
    MeshRenderer meshRenderer;
    Rigidbody rb;

    void PowerupGained()
    {
        //On collision with the powerup, destroy the powerup so a loop canÅft be exploited
        //After the powerup has been destroyed, this method will boost a multiplier based on the powerup gained;
        //maybe a switch-case could be useful for determining the buff?
    }
    void UpdateStats() {
        attackSpeed = attackSpeedMultiplier;
        toughness = (int) (10 * toughnessBreakMultiplier);
        for (int i = 0; i < attackHitboxes.Length; i++) {
            attackHitboxes[i].attackPower = (int)(15 * attackMultiplier);
        }
        
        health = (int)(100 * maxHPMultiplier);

    }

    //Player will get hit by enemy hitboxes
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Hitbox attackerHitbox = other.gameObject.GetComponent<Hitbox>();
            health -= attackerHitbox.attackPower;
        }
    }
    private void Start()
    {
        movementScript = GetComponent<MovementScript>();
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (health <= 0) {
            movementScript.isDead = true;
            meshRenderer.enabled = false;
        }
    }
}
