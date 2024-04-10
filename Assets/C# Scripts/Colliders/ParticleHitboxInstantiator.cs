using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleHitboxInstantiator : MonoBehaviour
{
    [SerializeField] public GameObject hitbox;
    private ParticleSystem attackParticleSystem;
    ParticleSystem.Particle[] particles;
    List<GameObject> spawnedHitboxes = new List<GameObject>();
    int numParticlesAlive;

    private void Awake()
    {
        attackParticleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[attackParticleSystem.main.maxParticles];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //numParticlesAlive = attackParticleSystem.GetParticles(particles);
        //Debug.Log(numParticlesAlive);
        //if (numParticlesAlive > 0)
        //{
        //    for (int i = 0; i < numParticlesAlive; i++)
        //    {
        //        StartCoroutine(InstantHitbox(particles[i].position + attackParticleSystem.transform.position, particles[i].remainingLifetime));
        //    }
        //}

        // Spawn / Destroy hitboxes to match live particles
        StartCoroutine(RegulateParticleMapping());
        // Once there are hitboxes, move them with the particles
        MoveMappedParticles();
    }

    IEnumerator InstantHitbox(Vector3 particlePosition, float particleLifetime) {
        Debug.Log("Calling InstantHitbox");
        GameObject instantHitbox = Instantiate(hitbox, particlePosition, Quaternion.identity);
        yield return new WaitForSeconds(particleLifetime);
        Debug.Log("Destroying InstantHitbox");
        Destroy(instantHitbox);
        yield break;
    }

    IEnumerator RegulateParticleMapping()
    {
        numParticlesAlive = attackParticleSystem.GetParticles(particles);
        GameObject hitboxReference = null;
        // If particle numbers do not match hitbox numbers, correct this
        while (numParticlesAlive > spawnedHitboxes.Count)
        {
            hitboxReference = Instantiate(hitbox, attackParticleSystem.transform.position,
            Quaternion.identity);
            spawnedHitboxes.Add(
            hitboxReference
            );
            Debug.Log("Spawning particles");
        }

        yield return new WaitForSeconds(attackParticleSystem.main.startLifetimeMultiplier);

        while (numParticlesAlive < spawnedHitboxes.Count)
        {
            Destroy(spawnedHitboxes[spawnedHitboxes.Count-1]);
            spawnedHitboxes.RemoveAt(spawnedHitboxes.Count - 1);
        }

        yield return null;
    }

    void MoveMappedParticles()
    {
        numParticlesAlive = attackParticleSystem.GetParticles(particles);

        // If there is an inbalance between hitboxes and particles, do nothing
        if (numParticlesAlive != spawnedHitboxes.Count) return;

        // Move hitboxes with particles
        for (int i = 0; i < numParticlesAlive; i++)
        {
            ParticleSystem.Particle particle = particles[i];

            if(particle.remainingLifetime > 0)
            {
                spawnedHitboxes[i].transform.position = attackParticleSystem.transform.localPosition + particle.position;
            }
        }
    }
}
