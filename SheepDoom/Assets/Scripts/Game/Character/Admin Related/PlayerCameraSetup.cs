using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class PlayerCameraSetup : NetworkBehaviour
    {
        // camera instantiated only on local client
        [SerializeField] private GameObject PlayerCamera;
        [SerializeField] private GameObject createdCam;

        public GameObject P_createdCam
        {
            get{return createdCam;}
            set{createdCam = value;}
        }

        public override void OnStartAuthority()
        {
            createdCam = Instantiate(PlayerCamera);
            createdCam.GetComponent<CameraFollow>().player = gameObject;
        }

        public override void OnStopClient()
        {
            Destroy(createdCam);
        }
    }
}
