using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class GetGameStatus : MonoBehaviour
    {
        //[SerializeField] GameObject gameStatus;
        [SerializeField] private GameObject MapObjects;
        [SerializeField] private GameObject GameCanvas;

        public void Disable_MapObjects()
        {
            MapObjects.SetActive(false);
        }

        public void Disable_GameCanvas()
        {
            GameCanvas.SetActive(false);
        }
    }
}