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
        private Vector3 wantedPosition = new Vector3(0, 0, 10f);
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

/*
//setting owner
public GameObject owner;
public Vector3 ownerTransform;

//duration
[SerializeField] private float duration, durationInGame;

public void setOwner(GameObject objectOwner)
{
    owner = objectOwner;
}

private void Start()
{
    durationInGame = duration;
    ownerTransform = owner.gameObject.transform.position;
}
// Update is called once per frame
void Update()
{
    // object position is based of owner position + set distance
    this.gameObject.transform.position = owner.gameObject.transform.position + owner.transform.TransformDirection(0, 0, 15);
    this.gameObject.transform.LookAt(owner.gameObject.transform);


    //timers
    durationInGame -= Time.deltaTime;

    if (durationInGame <= 0)
    {
        Destroy(this.gameObject);
    }
}*/
