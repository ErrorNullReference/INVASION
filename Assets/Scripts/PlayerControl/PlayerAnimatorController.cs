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
    Vector3 dir, oldPos, cameraDir, playerDirection;
    Camera camera;
    Animator animator;

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
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            float angle = -Mathf.Deg2Rad * Vector3.SignedAngle(new Vector3(x, 0, z).normalized, transform.forward, Vector3.up);
            dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            if (Input.GetButton("Sprint"))
                dir *= 2;
        }
    }

    void ExtrapolateDirectionWithoutInputs()
    {
        playerDirection = transform.position - oldPos;
        oldPos = transform.position;

        float magnitude = playerDirection.magnitude / Time.deltaTime;
        if (magnitude >= movementTreshold)
        {
            float angle = -Mathf.Deg2Rad * Vector3.SignedAngle(playerDirection.normalized, transform.forward, Vector3.up);
            dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            if (magnitude >= RunTreshold)
                dir *= 2;
        }
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
