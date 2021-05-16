using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class BuffFollowScript : NetworkBehaviour
    {
        //duration
        [SerializeField] private float duration, durationInGame;

        private Vector3 relativePosition;
        private Vector3 wantedPosition = new Vector3(0, 0, 1.5f);
        [SyncVar] public GameObject owner;
        public Transform ownerTransform;
        [SyncVar] public float teamID;

        private void Start()
        {
            ownerTransform = owner.transform;
            durationInGame = duration;
            teamID = owner.gameObject.GetComponent<PlayerAdmin>().getTeamIndex();
        }

        private void Update()
        {
            relativePosition = ownerTransform.TransformPoint(wantedPosition);

            this.gameObject.transform.position = relativePosition;
            this.gameObject.transform.LookAt(ownerTransform);

            //timers
            durationInGame -= Time.deltaTime;

            if (durationInGame <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
}