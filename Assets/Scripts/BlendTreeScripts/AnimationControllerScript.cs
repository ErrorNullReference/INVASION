using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class AnimationControllerScript : MonoBehaviour
{
    public AnimatorPropertyHolder AnimSpeed;
    public AnimatorPropertyHolder LeftSpeed;
    public AnimatorPropertyHolder speed;
    private Animator anim;

    public float SpeedForward
    {
        get { return anim.GetFloat(AnimSpeed.PropertyHash); }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Animation(float forward, float right)
    {
        anim.SetFloat((int)speed, forward);
        anim.SetFloat((int)LeftSpeed, right);
    }
}

