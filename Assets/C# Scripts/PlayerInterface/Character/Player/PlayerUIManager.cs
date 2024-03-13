using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{

    public static PlayerUIManager instance;
    [SerializeField] bool StartGameAsClient;

    [HideInInspector] public PlayerUIHUDManager playerUIHUDManager;
    [HideInInspector] public PlayerUIPopupManager playerUIPopUpManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Debug.Log("Dupe detected");
            Destroy(gameObject);
        }

        playerUIHUDManager = GetComponentInChildren<PlayerUIHUDManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopupManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (StartGameAsClient)
        {
            StartGameAsClient = false;
            // Shut down the network to connect as a client
            NetworkManager.Singleton.Shutdown();
            // Restart the network as a client
            NetworkManager.Singleton.StartClient();
        }
    }
}
