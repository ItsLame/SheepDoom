using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public Animator transition;
    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void FadeOut()
    {
        transition.SetTrigger("FadeOut");
    }
}
