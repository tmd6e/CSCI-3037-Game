using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Attacker")]
    public CharacterManager attacker;

    [Header("Damage")]
    public float physDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;
    [Header("Final Damage")]
    public int finalDamage = 0;

    [Header("Toughness")]
    public float toughnessDamage = 0;
    public bool toughnessBroken = false; // If broken, stuns and plays damage animation

    // Status effect buildups

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementSoundFX; // Plays if elemental damage is present

    [Header("Direction Damage Taken From")]
    public float angleHitFrom; // What animation to play based on value
    public Vector3 contactPoint; // Where to instantiate damage FX

    public override void ProcessEffect(CharacterManager character) { 
        base.ProcessEffect(character);

        // Don't process the damage if the character is dead
        if (character.isDead.Value) {
            return;
        }
        // Check for i-frames

        // Calculate damage
        CalculateDamage(character);
        // Check direction of damage

        // Play damage animation

        // Check for status effect build-up

        // Play damage sound fx

        // Play damage vfx

        // If character is an AI, check for new target if attacker is present
    }

    private void CalculateDamage(CharacterManager character) {
        if (!character.IsOwner) {
            return;
        }
        if (attacker != null) { 
            // Modify base damage based on powerups and attributes
        }

        // Check character's flat defense from powerups

        // Check character's defense multipliers and subtract percentage from damage

        // Add damage types together for final damage
        finalDamage = Mathf.RoundToInt(physDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        // Consider if the character has toughness
        if (character.characterNetworkManager.canBeBroken.Value) {
            if (character.characterNetworkManager.currentToughness.Value <= 0) {
                finalDamage *= 2;
            }
            else {
                finalDamage /= 2;
            }
        }

        if (finalDamage <= 0) {
            finalDamage = 1;
        }

        character.characterNetworkManager.currentHealth.Value -= finalDamage;
        if (character.characterNetworkManager.canBeBroken.Value)
        {
            character.characterNetworkManager.currentToughness.Value -= toughnessDamage;
        }
    }

}
