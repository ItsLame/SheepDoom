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
        [Server]
        public void setOwner(GameObject theOwner)
        {
            owner = theOwner;
        }

        void Start()
        {
            if(isClient && hasAuthority)
                ownerTransform = owner.gameObject.transform.position;
        }

        [Command]
        void CmdDestroy()
        {
            NetworkServer.Destroy(gameObject);
        }

        void Update()
        {
            //destroy object if owner position change
           if(isClient && hasAuthority)
           {
                if ((Vector3.Distance(ownerTransform, owner.gameObject.transform.position) > 5))
                    CmdDestroy();
           }
        }
    }
}

