using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SheepDoom
{
    public class rotation_script : MonoBehaviour
    {
        public bool hasCapturers;


        //rotation controls
        public float x_rotaspeed;
        public float y_rotaspeed;
        public float z_rotaspeed;
        // Start is called before the first frame update


        // Update is called once per frame
        void Update()
        {

            if (!hasCapturers)
            {
                transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);
            }

            if (hasCapturers)
            {
                transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed * 10, 1.0f * z_rotaspeed);
            }

        }
    }

}
