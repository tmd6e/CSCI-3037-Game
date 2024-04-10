using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterGameObject;
    [SerializeField] GameObject instantiatedCharacter;

    private void Awake()
    {
        
    }

    private void Start()
    {
        WorldAIManager.instance.SpawnCharacter(this);
        gameObject.SetActive(false);
    }

    public void AttemptToSpawnCharacter() {
        if (characterGameObject != null) {
            instantiatedCharacter = Instantiate(characterGameObject);
            instantiatedCharacter.transform.position = transform.position;
            instantiatedCharacter.transform.rotation = transform.rotation;
            instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
        }
    }
}
