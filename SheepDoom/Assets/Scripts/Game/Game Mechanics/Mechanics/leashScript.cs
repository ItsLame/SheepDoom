using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class leashScript : MonoBehaviour
    {
        [Header("Starting position")]
        [SerializeField] private Transform startPos;
        [SerializeField] private GameObject parent;


        private void Start()
        {
            parent = this.gameObject.GetComponent<GetParents>().getParent();            
        }

        [Server]
        public void backToStart()
        {
            parent.transform.position = startPos.transform.position;
        }
    }
}

