using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class AnimationControllerScript : MonoBehaviour
{
    public AnimatorPropertyHolder AnimSpeed;
    public AnimatorPropertyHolder SpeedY, SpeedX, SpeedMag;
    private Animator anim;

    public float SpeedForward
    {
        get { return anim.GetFloat(AnimSpeed); }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Animation(float speedX, float speedY, float magnitude)
    {
        if (anim == null)
            return;
        anim.SetFloat(SpeedMag.PropertyName, magnitude);
        anim.SetFloat(SpeedY.PropertyName, speedX);
        anim.SetFloat(SpeedX.PropertyName, speedY);
    }

    public void Animation(Vector3 forward, Vector3 direction, float magnitude)
    {
        if (anim == null)
            return;

        float ang = Vector3.SignedAngle(transform.forward, direction, Vector3.up) * Mathf.Deg2Rad;

        anim.SetFloat(SpeedMag.PropertyName, magnitude);
        anim.SetFloat(SpeedY.PropertyName, Mathf.Cos(ang));
        anim.SetFloat(SpeedX.PropertyName, Mathf.Sin(ang));
    }
}

