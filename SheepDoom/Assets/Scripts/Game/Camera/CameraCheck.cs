using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraCheck : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
            gameObject.SetActive(true);
        else if (isClient)
            gameObject.SetActive(false);
    }
}
