using System;

public class Entity
{
    public int Health { get; set; }

    // Constructor
    public Entity(int health = 100)
    {
        Health = health;
    }

    // Actions
    public virtual void Damage(int damage)
    {
        health -= damage;
    }
}
