using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    public GameObject OptionsUI;
    public GameObject CreditsUI;
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

    //literally what they mean
    public void OpenOptions()
    {
        OptionsUI.SetActive(true);
    }

    public void CloseOptions()
    {
        OptionsUI.SetActive(false);
    }

    public void OpenCredits()
    {
        CreditsUI.SetActive(true);
    }

    public void CloseCredits()
    {
        CreditsUI.SetActive(false);
    }
}
