using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CustomRigidBody))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float WalkSpeed, RunSpeed;
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
            dir = camera.transform.forward * v + camera.transform.right * h;
        else
            dir = Vector3.forward * v + Vector3.right * h;
        dir.y = 0;

        Move(dir, Input.GetButtonDown("Sprint"));
        Rotate();
    }

    void Move(Vector3 direction, bool run)
    {
        body.velocity = direction;
        body.MovePosition(transform.position + direction * Time.deltaTime * (run ? RunSpeed : WalkSpeed));
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
    }

    /*public float Speed, RunSpeed;
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
            body.Move(dir, (Input.GetButton("Sprint") ? RunSpeed : Speed), Time.deltaTime, false);
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
    }*/
}
