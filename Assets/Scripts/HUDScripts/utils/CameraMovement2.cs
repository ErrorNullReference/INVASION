using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement2 : MonoBehaviour
{

    public Transform target;
    public float smoothSpeed;
    public Vector3 offset;
    public Vector3 zoomIn;
    

    void LateUpdate()
    {
        transform.LookAt(target);
        Vector3 newpos = target.position + offset;
        Vector3 newCameraPos = Vector3.Lerp(transform.position, newpos, smoothSpeed * 1.5f);
        transform.position = newCameraPos;

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {

            Camera.main.fieldOfView++;
            if (Camera.main.fieldOfView >= 70)
            {
                Camera.main.fieldOfView = 70;
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.fieldOfView--;
            if (Camera.main.fieldOfView <= 45)
            {
                Camera.main.fieldOfView = 45;
            }
        }
    }
}

