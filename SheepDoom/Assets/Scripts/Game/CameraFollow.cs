using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private Vector3 cameraoffset;

    [Range(0.01f, 1.0f)]
    public float smoothness = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        cameraoffset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos1 = player.position + cameraoffset;
        transform.position = Vector3.Slerp(transform.position, newPos1, smoothness);
        
    }
}
