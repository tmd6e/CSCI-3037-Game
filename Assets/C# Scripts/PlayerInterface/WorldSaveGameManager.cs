using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    [SerializeField] int worldSceneIndex = 1;

    public void Awake()
    {
        // Only one manager at a time
        if (instance == null)
        {
            instance = this;
        }
        else { 
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public IEnumerator LoadNewGame() {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);
        yield return null;
    }

    public int GetWorldSceneIndex() { 
        return worldSceneIndex;
    }
}
