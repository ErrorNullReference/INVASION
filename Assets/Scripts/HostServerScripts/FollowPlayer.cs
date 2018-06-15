using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    Transform Target;
    public Vector3 Offset;
    public Vector3 Rotation;

    void Start()
    {
        Target = FindObjectOfType<PlayerController>().transform;   
        transform.rotation = Quaternion.Euler(Rotation);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target != null)
            transform.position = Target.position + Offset;
    }
}
