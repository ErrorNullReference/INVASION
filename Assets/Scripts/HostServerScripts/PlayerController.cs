using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomRigidBody))]
public class PlayerController : MonoBehaviour
{
    public float Speed, RunSpeed;
    public Camera Camera;
    CustomRigidBody body;
    RaycastHit hitInfo;

    void Start()
    {
        if (Camera == null)
            Camera = Camera.main;
        body = GetComponent<CustomRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveBody();
        SetDirection();
    }

    void MoveBody()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = Camera.transform.forward * z + Camera.transform.right * x;
        dir.y = 0;
        dir = dir.normalized;

        if (dir.sqrMagnitude != 0)
            body.Move(dir, (Input.GetButton("Sprint") ? RunSpeed : Speed), Time.deltaTime, true);
    }

    void SetDirection()
    {
        if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100))
            transform.LookAt(new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z));
    }
}
