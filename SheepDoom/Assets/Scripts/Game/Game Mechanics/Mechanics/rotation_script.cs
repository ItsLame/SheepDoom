using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace SheepDoom
{
    public class rotation_script : NetworkBehaviour
    {
        [SerializeField] private bool isHealth;
        [SerializeField] private GameObject owner;
        [SerializeField] private float num;

        //rotation controls
        [SerializeField] private float x_rotaspeed;
        [SerializeField] private float y_rotaspeed;
        [SerializeField] private float z_rotaspeed;
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            if (isServer)
            {
                if (!isHealth)
                {
                    num = owner.GetComponent<CapturePointScript>().getNumOfCapturers();
                }

                if (num == 0)
                {
                    transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed, 1.0f * z_rotaspeed);
                }

                if (num != 0)
                {
                    transform.Rotate(1.0f * x_rotaspeed, 1.0f * y_rotaspeed * 30, 1.0f * z_rotaspeed);
                }
            }
        }
    }
}
