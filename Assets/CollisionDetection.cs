using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController wc;
    //public GameObject HitParticle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && wc.IsAttacking)
        {
            /* Animation pseudocode
             * other.GetComponent<Animator>().SetTrigger("Hit");
             */
            Debug.Log("Collision Detected with:" + other.name);
            //Instantiate(HitParticle, new Vector3(other.transform.position.x, 
            //transform.position.y, other.transform.position.z), 
            //other.transform.rotation);
            Entity entityScript = other.GetComponent<Entity>();
            Debug.Log("Retrieving entity script");
            if (entityScript != null)
            {
                entityScript.TakeDamage(25);
                Debug.Log("Took 25 damage");
            }
            else
            {
                Debug.Log("EnemyScript = null");
            }
        }
    }
}
