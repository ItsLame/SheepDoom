using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class ChannelingScript : NetworkBehaviour
    {
        //setting owner
        [SerializeField] [SyncVar] private GameObject owner;
        [SerializeField] private Vector3 ownerTransform;

        //set method
        public void setOwner(GameObject theOwner)
        {
       //     Debug.Log(theOwner.name + " set to channel's owner");
            owner = theOwner;
        }

        void Start()
        {
            ownerTransform = owner.gameObject.transform.position;
        }


        [ServerCallback]
        void Update()
        {
            //destroy object if owner position change
            if ((Vector3.Distance(ownerTransform, owner.gameObject.transform.position) > 5))
            {
                Destroy(this.gameObject);
            }
        }
    }
}

