using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class StartAnimatingScript : NetworkBehaviour
{
    public override void OnStartServer()
    {
        gameObject.GetComponent<NetworkAnimator>().SetTrigger("StartMoving");
    }
}
