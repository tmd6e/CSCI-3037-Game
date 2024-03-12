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
    private void Start()
    {
        
    }
    private void Update()
    {
    }
}
