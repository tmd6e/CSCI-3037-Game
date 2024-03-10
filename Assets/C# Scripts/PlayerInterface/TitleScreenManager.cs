using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TitleScreenManager : MonoBehaviour
{
    public void StartNetworkAsHost() {
        Debug.Log("This function is active");
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame() { 
       StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());
    }
}
