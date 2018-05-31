using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerTwo : MonoBehaviour {

    public float speed;


    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal_Key"), 0, Input.GetAxis("Vertical_Key")) * speed * Time.deltaTime;

    }
}
