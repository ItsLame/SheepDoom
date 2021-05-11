using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;
    public float fixXdist = -270.0f;
    public float fixYdist = 266.5f;
    public float fixZdist = 22.0f;

    [Range(0.01f, 1.0f)]
    public float smoothness = 0.5f;

    // Update is called once per frame
    void Update()
    {
        // to ignore update when victory
        if(this.gameObject.GetComponent<CamSwitchManager>().isVictory)
            return;

        //if player is still alive
        if (player != null)
        {
            transform.position = new Vector3(player.transform.position.x + fixXdist, player.transform.position.y + fixYdist, player.transform.position.z + fixZdist);
        }
    }
}
