using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    public InputField username;
    public InputField password;

    void Start()
    {
        
        if (!Application.isBatchMode)
        { //Headless build
            Debug.Log($"=== Client Build ===");
            
        }
        else
        {
            Debug.Log($"=== Server Build ===");
            networkManager.StartServer();
        }  
    }

    public void Login() 
    {
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    public void Register() { }
}
