using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 Direction;
    public float Speed = 1;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Direction * Speed * Time.deltaTime);
    }
}
