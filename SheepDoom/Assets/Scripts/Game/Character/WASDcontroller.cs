using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDcontroller : MonoBehaviour
{
    public float speed;
    public Camera mainCamera;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.D))
            pos.z -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
            pos.z += speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            pos.x -= speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            pos.x += speed * Time.deltaTime;

        transform.position = pos;

        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        //Ray cameraRay = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }
}
