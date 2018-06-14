using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAnimation : MonoBehaviour {

    public void Play()
    {
        Animation anim = GetComponent<Animation>();
        anim.Play();
    }
}
