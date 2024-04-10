using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGolemCombatManager : AICharacterCombatManager
{
    public AudioClip themeMusic;
    public bool fightTriggered = false;
    public void Update()
    {
        if (currentTarget != null && !fightTriggered && themeMusic != null) {
            fightTriggered = true;
            WorldSoundFXManager.instance.globalAudioSource.clip = themeMusic;
            WorldSoundFXManager.instance.globalAudioSource.Play();
        }
    }
}
