using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGolemCombatManager : AICharacterCombatManager
{
    public AudioClip themeMusic;
    public AudioClip phase2Music;
    public bool fightTriggered = false;
    public void Update()
    {
        if (currentTarget != null && !fightTriggered && themeMusic != null) {
            fightTriggered = true;
            WorldSoundFXManager.instance.globalAudioSource.clip = themeMusic;
            WorldSoundFXManager.instance.globalAudioSource.Play();
        }

        if (character.characterNetworkManager.currentHealth.Value <= (character.characterNetworkManager.maxHealth.Value/2) && !phase2Triggered && phase2Music != null) {
            Debug.Log("Playing Phase 2 music");
            phase2Triggered = true;
            WorldSoundFXManager.instance.globalAudioSource.clip = phase2Music;
            WorldSoundFXManager.instance.globalAudioSource.Play();
        }

        if (character.isDead.Value) {
            WorldSoundFXManager.instance.globalAudioSource.clip = WorldSoundFXManager.instance.overworldMusic;
            WorldSoundFXManager.instance.globalAudioSource.Play();
        }
    }
}
