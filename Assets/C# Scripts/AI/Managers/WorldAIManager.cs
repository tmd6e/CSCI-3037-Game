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
    public List<GameObject> spawnedCharacters;

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

    public void DespawnAllCharacters()
    {
        Debug.Log("Despawning all characters");
        foreach (var character in spawnedCharacters)
        {
            if (character.gameObject != null)
            {
                character.GetComponent<NetworkObject>().Despawn();
                Destroy(character.gameObject);
            }
        }
        spawnedCharacters.Clear();
    }
    public void ResetAllCharacters()
    {
        DespawnAllCharacters();
        if (NetworkManager.Singleton.IsServer)
        {
            foreach (var spawner in aICharacterSpawners)
            {
                spawner.ResetCharacter();
            }
        }
        
    }

    private void DisableAllCharacters()
    {
    }
}
