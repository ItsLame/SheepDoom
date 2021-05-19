using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class MessageProjectileScript : NetworkBehaviour
    {
        [SerializeField] private GameObject owner;
        [SerializeField] private float ownerTeamID;
        public override void OnStartServer()
        {
            ownerTeamID = owner.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
            Invoke("Destroyy", 2f);
        }

        [Server]
        public void SetOwnerProjectile(GameObject player)
        {
            owner = player;
        }

        [ServerCallback]
        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.CompareTag("Base"))
            {
                if (ownerTeamID == 2)
                {
                    if (col.gameObject.GetComponent<CaptureBaseScript>().getTeamID() == 1)
                    {
                        col.gameObject.GetComponent<CaptureBaseScript>().reduceNumOfCapturers();
                        Destroyy();
                    }
                }
                else if (ownerTeamID == 1)
                {
                    if (col.gameObject.GetComponent<CaptureBaseScript>().getTeamID() == 2)
                    {
                        col.gameObject.GetComponent<CaptureBaseScript>().reduceNumOfCapturers();
                        Destroyy();
                    }
                }
            }
        }

        [Server]
        private void Destroyy()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}

