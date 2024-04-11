using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;
    public AudioSource globalAudioSource;
    public AudioClip overworldMusic;

    [Header("Sound Effects")]
    public AudioClip music;
    public AudioClip rollSFX;

    private void Awake()
    {
        
        if (instance == null) { 
            instance = this;
            globalAudioSource = GetComponent<AudioSource>();
            globalAudioSource.clip = music;
            globalAudioSource.Play();
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
