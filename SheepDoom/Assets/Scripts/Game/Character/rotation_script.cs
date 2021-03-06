using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation_script : MonoBehaviour
{
    //rotation controls
    public float x_rotaspeed;
    public float y_rotaspeed;
    public float z_rotaspeed;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);

    }
}
