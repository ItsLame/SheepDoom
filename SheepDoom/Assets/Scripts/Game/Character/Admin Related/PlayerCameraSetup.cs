using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

    public class PlayerCameraSetup : NetworkBehaviour
    {
        // camera instantiated only on local client
        [SerializeField]
        private GameObject PlayerCamera;

        public override void OnStartAuthority()
        {
            GameObject createdCam = Instantiate(PlayerCamera);
            createdCam.GetComponent<CameraFollow>().player = gameObject;
        }
    }
