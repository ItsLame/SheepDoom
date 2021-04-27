using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNameUI : MonoBehaviour
{
    // Update is called once per frame
    private void LateUpdate()
    {
        if (Camera.main)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}
