using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;
    public AudioSource globalAudioSource;

    [Header("Sound Effects")]
    public AudioClip music;
    public AudioClip rollSFX;

    private void Awake()
    {
        globalAudioSource = GetComponent<AudioSource>();
        globalAudioSource.clip = music;
        globalAudioSource.Play();
        if (instance == null) { 
            instance = this;
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
