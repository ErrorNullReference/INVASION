using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothDamp : MonoBehaviour
{

    public Transform target;
    public Vector3 smoothSpeed;
    public Vector3 offset;
    public Vector3 zoomIn;
    public float maxSpeed;



    

    void LateUpdate()
    {
        transform.LookAt(target);
        Vector3 newpos =target.position+target.rotation* offset;
        //transform.localPosition = offset;
        transform.position = Vector3.SmoothDamp(transform.position, newpos, ref smoothSpeed, 1.5f, maxSpeed);         
    }
}

