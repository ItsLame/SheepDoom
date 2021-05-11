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

        // Update is called once per frame
        
        /*
        void Update()
        {
            if (gameStatus != null)
            {
                if (gameStatus.GetComponent<GameStatus>().P_gameEnded)
                    gameObject.SetActive(false);
            }
        }
        */

        public void Disable_MapObjects()
        {
            MapObjects.SetActive(false);
        }

        public void Disable_GameCanvas()
        {
            GameCanvas.SetActive(false);
        }

        /*
        public GameObject GetGameStatusManager()
        {
            return gameStatus;
        }
        */
    }
}