using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("DEBUG")]
    [SerializeField] bool despawnCharacters = false;
    [SerializeField] bool respawnCharacters = false;
    


    [Header("Characters")]
    [SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<GameObject> spawnedCharacters;

    private void Awake()
    {
        Debug.Log("Awaken!");
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log("Started!");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server");
            StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
        }
        else
        {
            Debug.Log("Not Server");
            //StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
        }
    }

    private void Update()
    {
       
        if (respawnCharacters)
        {
            respawnCharacters = false;
            DespawnAllCharacters();
            SpawnAllCharacters();
        }
        if (despawnCharacters)
        {
            despawnCharacters = false;
            DespawnAllCharacters();
        }
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacters()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }

        SpawnAllCharacters();
    }

    private void SpawnAllCharacters()
    {
        Debug.Log("Spawned!");
        foreach (var character in aiCharacters)
        {
            Debug.Log("Character!");
            GameObject instantiatedCharacter = Instantiate(character);
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
            spawnedCharacters.Add(instantiatedCharacter);
        }
    }

    private void DespawnAllCharacters()
    {
        foreach (var character in spawnedCharacters)
        {
            character.GetComponent<NetworkObject>().Despawn();
        }
        spawnedCharacters.Clear();
    }

    private void DisableAllCharacters()
    {
        // THIS IS A POTENTIAL OPTIMIZATION : NOT MANDATORY!

        // TO DO DISABLE CHARACTER GAMEOBJECTS, SYNC DISABLED STATUS ON NETWORK
        // DISABLE GAMEOBJECTS FOR CLIENTS UPON CONNECTING, IF DISABLED STATUS IS TRUE
        // CAN BE USED TO DISABLE CHARACTERS THAT ARE FAR FROM PLAYERS TO SAVE MEMORY
        // CHARACTERS CAN BE SPLIT INTO AREAS (AREA_00_, AREA_001_, etc)
    }
}
