using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

//[RequireComponent(typeof(CustomRigidBody))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public ReferenceFloat WalkSpeed, RunSpeed;
    public float MouseRotationOffset;
    public LayerMask Mask;
    Rigidbody body;
    Camera camera;
    RaycastHit hitInfo;
    bool active;

    public Vector3 Velocity { get { return body.velocity; } }

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();

        if (Camera.main != null)
            camera = Camera.main;

        MenuEvents.OnMenuOpen += Disable;
        MenuEvents.OnMenuClose += Activate;

        active = true;
    }

    void FixedUpdate()
    {
        if (!active)
            return;

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
        body.velocity = Vector3.zero;
        body.angularDrag = 0;
        body.angularVelocity = Vector3.zero;
       
        body.MovePosition(transform.position + direction * (run ? RunSpeed : WalkSpeed) * Time.deltaTime);
    }

    void Rotate()
    {
        body.angularVelocity = Vector3.zero;

        if (camera != null)
        {
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hitInfo, 100, Mask.value))
            {
                Vector3 dir = (hitInfo.point - transform.position).normalized;
                dir.y = 0;
                body.MoveRotation(Quaternion.LookRotation(dir));
            }
        }
        else
            transform.rotation = Quaternion.identity;
        
        /*if (camera != null)
        {
            Vector2 positionOnScreen = camera.WorldToViewportPoint(transform.position);
            Vector2 mouseOnScreen = (Vector2)camera.ScreenToViewportPoint(Input.mousePosition);
            float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen) + MouseRotationOffset;
            body.MoveRotation(Quaternion.Euler(0, -angle, 0));
        }
        else
            transform.rotation = Quaternion.identity;*/

    }

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    Vector3 GetCameraDirectionPerAxes(Camera camera, float horizontal, float vertical)
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, camera.transform.eulerAngles.y, 0));
        Vector3 forward = rotation * new Vector3(0, 0, 1);
        Vector3 right = rotation * new Vector3(1, 0, 0);
        return forward * vertical + right * horizontal;
    }

    void Disable()
    {
        active = false;
    }

    void Activate()
    {
        active = true;
    }

    void OnDestroy()
    {
        MenuEvents.OnMenuOpen -= Disable;
        MenuEvents.OnMenuClose -= Activate;
    }
}
