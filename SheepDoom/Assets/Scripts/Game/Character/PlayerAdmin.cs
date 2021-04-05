using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAdmin : NetworkBehaviour
{
    GameObject isItAutoPrivateOrPublic;

    //camera attached to player
    [SerializeField]
    private GameObject PlayerCamera;


    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        var createdCam = Instantiate(PlayerCamera);
        createdCam.GetComponent<CameraFollow>().player = this.gameObject;
        createdCam.SetActive(true);
        //PlayerCamera.gameObject.SetActive(true);
    }
}
