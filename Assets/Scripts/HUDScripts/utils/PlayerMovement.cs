using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public HeadsUpDisplay HUDInput;
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * speed * Time.deltaTime; 
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0, -100 * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(new Vector3(0, 100 * Time.deltaTime));
        } 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (HUDInput.AmmoMag > 0)
                HUDInput.AmmoMag--;
        }
        if (Input.GetKey(KeyCode.R))
        {
            //HUDInput.AmmoHeld -= Mathf.Abs(10 - HUDInput.AmmoMag);
            HUDInput.AmmoMag = 10;

        }
    }
}
