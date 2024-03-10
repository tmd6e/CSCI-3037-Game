using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public Hitbox[] attackHitboxes;
    public bool isStationary_ = true;
    private Animator animator;
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
        animator = this.gameObject.GetComponent<Animator>();
        lastTimeShot_ = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.health <= 0)
        {
            //Play death animation
            animator.SetBool("isDead", true);
        }
        if (objectDestructionFlag.activeSelf)
        {
            Destroy(this.gameObject);
        }
        if (toughnessMeter && toughness <= 0) {
            StartCoroutine(ParameterFrameChange("toughnessBroken"));
            
        }
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

    //Hit registration
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            PlayerHitbox attackerHitbox = other.gameObject.GetComponent<PlayerHitbox>();

            // If toughness is up, take reduced damage
            if (toughnessMeter && toughness > 0)
            {
                toughness -= attackerHitbox.PlayerReference.toughnessBreak;
                health -= (int)(attackerHitbox.attackPower * 0.1);
            }
            // When toughness is depleted, take double damage
            else if (toughnessMeter && toughness <= 0)
            {
                health -= (int)(attackerHitbox.attackPower * 2);
            }
            else {
                health -= attackerHitbox.attackPower;
            }
        }
    }

    private IEnumerator ParameterFrameChange(string parameter) {yield return new WaitForSeconds(2.0f);
    }
}
