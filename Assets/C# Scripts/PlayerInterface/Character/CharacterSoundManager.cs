using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    public AudioClip footstepSound;
    public AudioClip[] sounds;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip selectedSound) { 
        audioSource.clip = selectedSound;
        audioSource.PlayOneShot(selectedSound);
    }
}
