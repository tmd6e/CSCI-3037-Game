using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

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
}
