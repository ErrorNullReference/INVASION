using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControllerScript : MonoBehaviour
{
    private Animator anim;

    public float SpeedForward
    {
        get { return anim.GetFloat("AnimSpeed"); }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Animation(float forward, float right)
    {
        anim.SetFloat("speed", forward);
        anim.SetFloat("LeftSpeed", right);
    }
}

