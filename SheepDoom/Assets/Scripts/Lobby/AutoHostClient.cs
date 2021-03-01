using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    /*
    void Start()
    {
        if (!Application.isBatchMode)
        {
            Debug.Log("Client Connected");
            networkManager.StartClient();
        }

        else
        {
            Debug.Log("Server Starting");
        }
    }
    */
    public void JoinLocal() 
    {
        Debug.Log("Start Game Pressed");
        networkManager.networkAddress = "localhost";
        networkManager.StartHost();
        Debug.Log("Server started");
    }
}
