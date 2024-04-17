using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleHitboxInstantiator : MonoBehaviour
{
    public static List<GameObject> gameObjects = new List<GameObject>();
    [SerializeField] public GameObject hitbox;
    private ParticleSystem attackParticleSystem;
    ParticleSystem.Particle[] particles;
    List<GameObject> spawnedHitboxes = new List<GameObject>();
    int numParticlesAlive;
    [Header("Sounds")]
    private AudioSource audioSource;
    public AudioClip instantiateSound;
    public bool generateSoundOnce = false;
    [Header("Character")]
    [SerializeField] public CharacterManager characterManager; // Used for particle control on NPCs
    

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        attackParticleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[attackParticleSystem.main.maxParticles];
        characterManager = GetComponentInParent<CharacterManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        GameObject hitboxToRemove = null;
        // If particle numbers do not match hitbox numbers, correct this
        while (numParticlesAlive > spawnedHitboxes.Count)
        {
            // If this is the first instance of the particle and the boolean is on, play the sound
            // Check if the sound exists first
            if (instantiateSound != null)
            {
                if (generateSoundOnce && spawnedHitboxes.Count == 1)
                {
                    audioSource.PlayOneShot(instantiateSound);
                }
            // Otherwise, if the boolean is off, play the sound each instance
                if (!generateSoundOnce) { 
                    audioSource.PlayOneShot(instantiateSound);
                }
            }
            hitboxReference = Instantiate(hitbox, attackParticleSystem.transform.position,
            Quaternion.identity);
            hitboxReference.transform.SetParent(attackParticleSystem.transform, false);
            spawnedHitboxes.Add(
            hitboxReference
            );
            gameObjects.Add(hitboxReference);
            Debug.Log("Spawning particles");
        }

        yield return new WaitForSeconds(attackParticleSystem.main.startLifetimeMultiplier);

        while (numParticlesAlive < spawnedHitboxes.Count)
        {
            hitboxToRemove = spawnedHitboxes[spawnedHitboxes.Count - 1];
            Destroy(hitboxToRemove);
            spawnedHitboxes.RemoveAt(spawnedHitboxes.Count - 1);
            gameObjects.Remove(hitboxToRemove);
        }

        yield return null;
    }

    void MoveMappedParticles()
    {
        numParticlesAlive = attackParticleSystem.GetParticles(particles);

        // If there is an inbalance between hitboxes and particles, do nothing
        if (numParticlesAlive != spawnedHitboxes.Count)
        {
            Debug.Log("IMBALANCE DETECTED, DO NOT MOVE");
            return;
        }

        // Move hitboxes with particles
        for (int i = 0; i < numParticlesAlive; i++)
        {
            ParticleSystem.Particle particle = particles[i];

            if (particle.remainingLifetime > 0)
            {
                spawnedHitboxes[i].transform.position = attackParticleSystem.transform.localPosition + particle.position;
                
            }
        }
    }

    static public void RemoveAllHitboxes()
    {
        foreach (var instance in gameObjects) { 
            Destroy(instance);
        }
        gameObjects.Clear();
    }
}
