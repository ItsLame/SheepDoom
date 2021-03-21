using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerOnMinimap : MonoBehaviour
{
    [SerializeField] private Camera PlayerMiniMapCamera;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(PlayerMiniMapCamera.transform);
    }
}
