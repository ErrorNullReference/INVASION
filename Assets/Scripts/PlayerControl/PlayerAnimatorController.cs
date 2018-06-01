using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]
    bool UseInputs;
    [SerializeField]
    string speedZ, speedX, shoot, death;
    [SerializeField]
    float RunTreshold, movementTreshold;
    int speedZHash, speedXHash, shootHash, deathHash;
    Animator animator;
    Vector3 dir, oldPos, cameraDir, playerDirection;
    Camera camera;

    void Start()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();
        speedZHash = Animator.StringToHash(speedZ);
        speedXHash = Animator.StringToHash(speedX);
        shootHash = Animator.StringToHash(shoot);
        deathHash = Animator.StringToHash(death);
    }

    // Update is called once per frame
    void Update()
    {
        dir = Vector3.zero;
            
        if (UseInputs)
        {
            ExtrapolateDirectionWithInputs();
            if (Input.GetButtonDown("Fire1"))
                Shoot();
        }
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
            cameraDir = camera.transform.forward * x + camera.transform.right * z;
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
        playerDirection = transform.position - oldPos;
        oldPos = transform.position;

        float magnitude = playerDirection.magnitude;
        if (magnitude >= movementTreshold)
        {
            playerDirection = playerDirection.normalized;
            cameraDir = camera.transform.forward * playerDirection.x + camera.transform.right * -playerDirection.z;
            cameraDir.y = 0;
            cameraDir = cameraDir.normalized;

            float ang = Mathf.Deg2Rad * Vector3.SignedAngle(cameraDir, transform.forward, Vector3.up);
            dir = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang));

            if (magnitude >= RunTreshold)
                dir *= 2;
        }
        Debug.Log(magnitude);
    }

    public void Shoot()
    {
        animator.SetTrigger(shootHash);
    }

    public void Die()
    {
        animator.SetTrigger(deathHash);
    }
}
