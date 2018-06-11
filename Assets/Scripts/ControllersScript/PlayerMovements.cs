using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [Range(0, 10)]
    public float Smooth;

    public float speed;

   
    // Update is called once per frame
    void Update()
    {
        //MOVEMENTS AND ROTATION
        Vector2 stick = Move();
        if (stick.magnitude <= 0.3f)
        {
            stick = Vector2.zero;
            
        }
           

        Vector2 RStick = MoveRightStick();
        if (RStick.magnitude <= 0.3f)
            RStick = Vector2.zero;

        //if (stick.magnitude != 0)
            UpdateForward(stick);

        transform.Translate(stick.x * Time.deltaTime * speed, 0, stick.y * Time.deltaTime * speed, Space.World);
        Vector3 direction = Vector3.right * RStick.x + Vector3.forward * RStick.y;

        if (direction.sqrMagnitude > 0)
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        //FIRE INPUT
        if (Input.GetKeyDown(KeyCode.Joystick1Button5))
        {
            Debug.Log("FIRE");
        }
        else if(Input.GetKey(KeyCode.Joystick1Button5))
        {
            Debug.Log("BURST");
        }

        //REALOD INPUT
        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            Debug.Log("RELOADING...");
        }
        //INTERACT INPUT
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            Debug.Log("INTERACT");
        }

    }

    private Vector2 Move()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
    }

    private Vector2 MoveRightStick()
    {
        return new Vector2(Input.GetAxis("RS_Horizontal"), Input.GetAxis("RS_Vertical"));
    }







    private void UpdateForward(Vector2 stick)
    {

        Vector3 newFor = Camera.main.transform.right * stick.x + Camera.main.transform.forward * stick.y;
        newFor.y = 0;
        transform.forward = Vector3.Lerp(transform.forward, newFor, Smooth * Time.deltaTime);
    }
}
