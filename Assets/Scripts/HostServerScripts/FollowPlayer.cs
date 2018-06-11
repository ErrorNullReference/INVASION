using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    Transform Target;
    public Vector3 Offset;

    void Start()
    {
        Target = FindObjectOfType<PlayerController>().transform;    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target != null)
            transform.position = Target.position + Offset;
    }
}
