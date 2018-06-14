using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private AnimatorPropertyHolder chase;
    [SerializeField]
    private bool Chase;
    private bool oldChase;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Chase = false;
        oldChase = Chase;
    }

    void Update()
    {
        if (Chase != oldChase)
        {
            if (Chase)
            {
                animator.SetBool((int)chase, true);
                oldChase = Chase;

            }
            else if (!Chase)
            {
                animator.SetBool((int)chase, false);
                oldChase = Chase;

            }
        }
    }

    public void SetChase(bool chase)
    {
        Chase = chase;
    }
}
