using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class ObjectFollowScript : NetworkBehaviour
    {
        //duration
        [SerializeField] private float duration, durationInGame;

        private Vector3 relativePosition;
        private Vector3 wantedPosition;
        [SyncVar] public GameObject owner;
        public Transform ownerTransform;
        [SyncVar] public float teamID;

        private void Start()
        {
            if(isServer)
            {
                durationInGame = duration;
                teamID = owner.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
            }
               
            if (isClient && hasAuthority)
            {
                if (CompareTag("Shield"))
                    wantedPosition = new Vector3(0, 0, 10f);
                else if (CompareTag("Skill"))
                    wantedPosition = new Vector3(0, 0, 1.5f);
                ownerTransform = owner.transform;
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
            //timers
            if(isServer)
            {
                durationInGame -= Time.deltaTime;

                if (durationInGame <= 0)
                    NetworkServer.Destroy(gameObject);
            }
        }
    }
}