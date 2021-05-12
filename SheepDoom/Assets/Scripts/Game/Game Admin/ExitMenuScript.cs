using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject ExitMenu;
    public void OpenExitMenu()
    {
        ExitMenu.SetActive(true);
    }

    public void CloseExitMenu()
    {
        ExitMenu.SetActive(false);
    }
}
