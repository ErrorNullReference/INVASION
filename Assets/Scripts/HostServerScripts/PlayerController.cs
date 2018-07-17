using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

//[RequireComponent(typeof(CustomRigidBody))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public ReferenceFloat WalkSpeed, RunSpeed;
    Rigidbody body;
    Camera camera;
    RaycastHit hitInfo;

    public Vector3 Velocity { get { return body.velocity; } }

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();

        if (Camera.main != null)
            camera = Camera.main;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir;
        if (camera != null)
            dir = GetCameraDirectionPerAxes(camera, h, v);
        else
            dir = Vector3.forward * v + Vector3.right * h;
        dir.y = 0;

        Move(dir.normalized, Input.GetButton("Sprint"));
        Rotate();
    }

    void Move(Vector3 direction, bool run)
    {
        //body.velocity = Vector3.zero;
        //body.angularDrag = 0;
        //body.angularVelocity = Vector3.zero;
       
        body.MovePosition(transform.position + direction * (run ? RunSpeed : WalkSpeed) * Time.deltaTime);
    }

    void Rotate()
    {
        body.angularVelocity = Vector3.zero;

        if (camera != null)
        {
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100))
            {
                Vector3 dir = (hitInfo.point - transform.position).normalized;
                dir.y = 0;
                body.MoveRotation(Quaternion.LookRotation(dir));
            }
        }
        else
            transform.rotation = Quaternion.identity;
    }

    Vector3 GetCameraDirectionPerAxes(Camera camera, float horizontal, float vertical)
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, camera.transform.eulerAngles.y, 0));
        Vector3 forward = rotation * new Vector3(0, 0, 1);
        Vector3 right = rotation * new Vector3(1, 0, 0);
        return forward * vertical + right * horizontal;
    }
}
