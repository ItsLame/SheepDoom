using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class ChannelingScript : MonoBehaviour
    {
        //setting owner
        [SerializeField] private GameObject owner;
        [SerializeField] private Vector3 ownerTransform;

        //set method
        public void setOwner(GameObject theOwner)
        {
            owner = theOwner;
        }

        void Start()
        {
            ownerTransform = owner.gameObject.transform.position;
        }


        void Update()
        {
            //destroy object if owner position change
            if (ownerTransform != owner.gameObject.transform.position)
            {
                Destroy(this.gameObject);
            }
        }
    }
}

