using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]
    bool UseInputs;
    [SerializeField]
    AnimatorPropertyHolder speedZ, speedX, shoot, death;
    [SerializeField]
    float RunTreshold, movementTreshold;
    Vector3 dir, oldPos, cameraDir, playerDirection;
    Camera camera;
    Animator animator;
    bool active;

    void Start()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();
        active = true;

        MenuEvents.OnMenuOpen += Disable;
        MenuEvents.OnMenuClose += Activate;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
            return;

        dir = Vector3.zero;

        if (UseInputs)
        {
            ExtrapolateDirectionWithInputs();
            if (Input.GetButtonDown(fire1))
                Shoot();
        }
        else
            ExtrapolateDirectionWithoutInputs();

        animator.SetFloat(speedX, dir.x);
        animator.SetFloat(speedZ, dir.z);
    }

    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    private const string sprint = "Sprint";
    private const string fire1 = "Fire1";

    void ExtrapolateDirectionWithInputs()
    {
        float x = Input.GetAxis(horizontal);
        float z = Input.GetAxis(vertical);

        if (x != 0 || z != 0)
        {
            cameraDir = camera.transform.forward * z + camera.transform.right * x;
            cameraDir.y = 0;
            float angle = -Mathf.Deg2Rad * Vector3.SignedAngle(cameraDir.normalized, transform.forward, Vector3.up);
            dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

            if (Input.GetButton(sprint))
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
        animator.SetTrigger(shoot);
    }

    public void Die(bool isDead)
    {
        animator.SetBool(death, isDead);
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
