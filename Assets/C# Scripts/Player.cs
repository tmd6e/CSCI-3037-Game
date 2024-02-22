using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    //Unique stats to the player
    float attackSpeed = 1.0f;
    //Values offered by powerup
    float attackMultiplier = 1.0f;
    float attackSpeedMultiplier = 1.0f;
    float maxHPMultiplier = 1.0f;
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
        attackPower = (int) (15 * attackMultiplier);
        health = (int)(100 * maxHPMultiplier);

    }

    //Player will get hit by enemy hitboxes
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Entity referenceEntity = other.gameObject.GetComponent<Entity>();
            health -= referenceEntity.attackPower;
            Debug.Log("Hit");
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
