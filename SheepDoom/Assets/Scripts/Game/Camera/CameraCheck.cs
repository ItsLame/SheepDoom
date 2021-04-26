using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraCheck : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(isServer && isClient)
            this.gameObject.SetActive(false);
        else if(isServer)
            this.gameObject.SetActive(true);
        else
            this.gameObject.SetActive(false);
    }
}
