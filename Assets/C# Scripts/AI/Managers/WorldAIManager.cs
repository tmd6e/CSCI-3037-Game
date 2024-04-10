using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;



    [Header("Characters")]
    public List<AICharacterSpawner> aICharacterSpawners;
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

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        if (NetworkManager.Singleton.IsServer) {
            aICharacterSpawners.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttemptToSpawnCharacter();
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
