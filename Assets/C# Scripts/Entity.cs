using System;

public class Entity
{
    public int health { get; set; }
    public int attackPower { get; set; }
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

    // Actions
    public virtual void TakeDamage(int damage)
    {
        // Damage will be a placeholder until we figure out how to receive the attacker's attack power for proper damage receival
        health -= damage;
    }
    public virtual void DealDamage() { 
    }
}
