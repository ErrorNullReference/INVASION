using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    public string speedZ, speedX;
    public float RunTreshold;
    public bool UseInputs;
    int speedZHash, speedXHash;
    Animator animator;
    Vector3 dir, oldPos;
    Camera camera;

    void Start()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();
        speedZHash = Animator.StringToHash(speedZ);
        speedXHash = Animator.StringToHash(speedX);
    }

    // Update is called once per frame
    void Update()
    {
        dir = Vector3.zero;
            
        if (UseInputs)
            ExtrapolateDirectionWithInputs();
        else
            ExtrapolateDirectionWithoutInputs();
        
        animator.SetFloat(speedXHash, dir.x);
        animator.SetFloat(speedZHash, dir.z);
    }

    void ExtrapolateDirectionWithInputs()
    {
        float x = Input.GetAxis("Horizontal");
        float z = -Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            Vector3 cameraDir = camera.transform.forward * x + camera.transform.right * z;
            cameraDir.y = 0;
            cameraDir = cameraDir.normalized;

            float ang = Mathf.Deg2Rad * Vector3.SignedAngle(cameraDir, transform.forward, Vector3.up);
            dir = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));

            if (Input.GetButton("Sprint"))
                dir *= 2;
        }
    }

    void ExtrapolateDirectionWithoutInputs()
    {
        Vector3 playerDirection = transform.position - oldPos;
        oldPos = transform.position;

        if (playerDirection.x != 0 || playerDirection.z != 0)
        {
            float ang = Mathf.Deg2Rad * Vector3.SignedAngle(playerDirection.normalized, transform.forward, Vector3.up);
            dir = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));

            if (playerDirection.magnitude >= RunTreshold)
                dir *= 2;
        }
    }
}
