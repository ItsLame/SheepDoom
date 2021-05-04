﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class GetGameStatus : MonoBehaviour
    {
        [SerializeField] GameObject gameStatus;
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            if (gameStatus != null)
            {
                if (gameStatus.GetComponent<GameStatus>().P_gameEnded)
                    gameObject.SetActive(false);
            }
        }
    }
}