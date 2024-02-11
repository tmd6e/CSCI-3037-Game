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
    public virtual void Attack(Entity target, int damage)
    {
        target.Health -= damage;
    }
}