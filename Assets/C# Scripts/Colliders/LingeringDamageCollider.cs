using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LingeringDamageCollider : MonoBehaviour
{
    static float timer = 0;
    public float tickTime = 0.7f;
    [Header("Damage")]
    public float physDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;
    public float toughnessDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    private void Update()
    {
        while (timer < tickTime) { 
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CharacterManager defender = other.GetComponent<CharacterManager>();
        contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        // Check if we can damage this target based on friendly fire
        if (defender.gameObject.layer == this.gameObject.layer)
        {
            return;
        }

        // Check if target is blocking

        // Check for i-frames
        if (defender.characterNetworkManager.isInvulnerable.Value)
        {
            return;
        }
        // Damage
        if (timer >= tickTime)
        {
            DamageTarget(defender);
            timer = 0;
        }
    }

    protected virtual void DamageTarget(CharacterManager defender)
    {
        // Proc damage only once

        //if (charactersDamaged.Contains(defender)) {
        //    return;
        //}

        //charactersDamaged.Add(defender);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physDamage = physDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.toughnessDamage = toughnessDamage;
        damageEffect.contactPoint = contactPoint;

        defender.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }
}