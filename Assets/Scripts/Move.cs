using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    float SpeedMov = 3f;
    float SpeedRot = 3f;
	
	// Update is called once per frame
	void Update ()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        transform.position += transform.forward * y * SpeedMov;
        transform.eulerAngles +=  new Vector3(0f,  x * SpeedRot, 0);

    }
}
