using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class PlayerChild : NetworkBehaviour
    {
        //how long the child will be active for
        public float duration;
        public float durationInGame;
        public bool isDisabled = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (durationInGame >= 0)
            {
                Debug.Log(durationInGame);
                durationInGame -= Time.deltaTime;
            }

            else
            {
                isDisabled = true;
            }

            if (isDisabled)
            {
                disable();
            }
        }

        public void refreshDuration()
        {
            durationInGame = duration;
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            this.gameObject.GetComponent<BoxCollider>().enabled = true;
        }

        public void disable()
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
            isDisabled = false;
        }
    }
}

