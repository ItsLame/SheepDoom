using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{

    public class SelfBuffScript : NetworkBehaviour
    {
        //duration
        [SerializeField] private float duration, durationInGame;

        private Vector3 relativePosition;


        private Vector3 wantedPosition = new Vector3(0, 8, 1.5f);
        [SyncVar] public GameObject owner;
        public Transform ownerTransform;
        [SyncVar] public float teamID;
        [SerializeField] public int HealPerSecond;

        private void Start()
        {
            if (isServer)
            {
                durationInGame = duration;
                teamID = owner.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
            }

            if (isClient && hasAuthority)
                ownerTransform = owner.transform;
        }

        [ServerCallback]
        void OnTriggerStay(Collider col)
        {
            //if hit player
            if (col.gameObject.CompareTag("Player"))
            {
                if (col.gameObject == owner)
                    col.gameObject.GetComponent<PlayerHealth>().modifyinghealth(HealPerSecond);
            }
        }

        private void Update()
        {
            if(isClient && hasAuthority)
            {
                relativePosition = ownerTransform.TransformPoint(wantedPosition);

                this.gameObject.transform.position = relativePosition;
                this.gameObject.transform.LookAt(ownerTransform);
            }
            
            if(isServer)
            {
                //timers
                durationInGame -= Time.deltaTime;

                if (durationInGame <= 0)
                    NetworkServer.Destroy(gameObject);
            }
        }
    }
}
