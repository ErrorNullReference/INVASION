using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovements : MonoBehaviour {

    public Transform target;
    public Vector3 offSet;
    float speed;

    private void LateUpdate()
    {
        this.transform.position = target.position + offSet;
        transform.LookAt(target);
    }
}
