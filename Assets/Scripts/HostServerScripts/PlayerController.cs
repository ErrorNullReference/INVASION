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
    Vector3 screenMid;

    void Start()
    {
        if (Camera == null)
            Camera = Camera.main;
        body = GetComponent<CustomRigidBody>();
        screenMid = new Vector2(Screen.width / 2f, Screen.height / 2f);
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
        else
        {
            Vector3 dir = (Input.mousePosition - screenMid).normalized;
            dir.z = dir.y;
            dir.y = 0;
            transform.forward = dir;
        }
    }
}
