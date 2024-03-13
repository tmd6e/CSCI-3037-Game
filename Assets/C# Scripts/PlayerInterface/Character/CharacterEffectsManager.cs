using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    // Process instant effect (e.g. damage/healing)
    public virtual void ProcessInstantEffect(InstantCharacterEffect effect) {
        // Take an effect
        // Process the effect
        effect.ProcessEffect(character);
        
    }
    // Process timed effects (e.g. damage over time)

    // Process static effects (e.g. adding/removing buffs from powerups)
}
