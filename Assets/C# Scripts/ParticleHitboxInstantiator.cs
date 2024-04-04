using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleHitboxInstantiator : MonoBehaviour
{
    [SerializeField] public GameObject hitbox;
    private ParticleSystem attackParticleSystem;
    ParticleSystem.Particle[] particles;
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
        numParticlesAlive = attackParticleSystem.GetParticles(particles);
        Debug.Log(numParticlesAlive);
        if (numParticlesAlive > 0)
        {
            for (int i = 0; i < numParticlesAlive; i++)
            {
                StartCoroutine(InstantHitbox(particles[i].position + attackParticleSystem.transform.position, particles[i].remainingLifetime));
            }
        }
    }

    IEnumerator InstantHitbox(Vector3 particlePosition, float particleLifetime) {
        Debug.Log("Calling InstantHitbox");
        GameObject instantHitbox = Instantiate(hitbox, particlePosition, Quaternion.identity);
        yield return new WaitForSeconds(particleLifetime);
        Debug.Log("Destroying InstantHitbox");
        Destroy(instantHitbox);
        yield break;
    }
}
