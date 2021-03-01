using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//script to manage open and closing of UIs
public class UI_Manager : MonoBehaviour
{
    //options & credits canvases
    public GameObject OptionsUI;
    public GameObject CreditsUI;

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
