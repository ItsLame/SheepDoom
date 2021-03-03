using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour
{
    NavMeshAgent agent;

    public float rotatespeedmovement = 0.1f;
    float rotatevelocity;
    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        
    }

    // Update is called once per frame
    void Update()
    {
        //when pressing left mouse button
        if(Input.touchCount == 1)
        {
            RaycastHit hit;
            Touch touch = Input.GetTouch(0);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, Mathf.Infinity))
            {
                //have the player to move to raycast point
                agent.SetDestination(hit.point);

                //Rotation
                Quaternion rotationToLookAt = Quaternion.LookRotation(hit.point - transform.position);
                float rotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationToLookAt.eulerAngles.y, ref rotatevelocity, rotatespeedmovement * (Time.deltaTime * 5));

                transform.eulerAngles = new Vector3(0, rotationY, 0);
            }
        }
       
    }
}
