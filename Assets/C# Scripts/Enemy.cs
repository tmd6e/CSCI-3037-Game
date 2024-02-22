using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public bool isStationary_ = true;
    float timeToShoot_ = 1.0f; // In Seconds
    float lastTimeShot_;

    // Consider replacing with an Enum
    // 0 = Wander()
    // 1 = MoveToPlayer()
    // 2 = OrbitPlayer()
    public int movementType_ = 0;

    // Start is called before the first frame update
    void Start()
    {
       lastTimeShot_= Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStationary_) Move();
        if (Time.time - lastTimeShot_ > timeToShoot_)
        {
            lastTimeShot_= Time.time;
            // Shoot();
        }
    }
    
    void Shoot(Vector3 startPosition, Quaternion direction, float speed )
    {
        
    }

    void Move()
    {
        if (movementType_ == 0) Wander();
        else if (movementType_ == 1) MoveToPlayer();
        else if (movementType_ == 2) OrbitPlayer();
    }

    void OrbitPlayer()
    {
        // If distance <= some_distance
            // Orbit
        // Else
            // MoveToPlayer();
    }

    void MoveToPlayer()
    {
        // Get Player Position
        // Move toward it
    }

    void Wander()
    {
        // Basic wander algorithm
    }


}
