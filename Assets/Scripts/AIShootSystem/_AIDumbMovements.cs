using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created just for test the Shoot behaviour
/// </summary>
public class _AIDumbMovements : MonoBehaviour
{
    public float Speed;
    public float RotSpeed;
	void Update ()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * Speed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * RotSpeed;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }
}
