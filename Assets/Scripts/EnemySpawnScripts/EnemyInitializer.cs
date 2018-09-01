using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyInitializer : ScriptableObject
{
    [SerializeField]
    public GameObject Body;
    [HideInInspector]
    public GameObject BodyInstance;
    Animator Animator;
    AnimationControllerScript AnimatorController;

    public void Init(Enemy enemy, Transform root)
    {
        if (BodyInstance == null)
        {
            BodyInstance = Instantiate(Body, root);
            Animator = BodyInstance.GetComponent<Animator>();
            AnimatorController = BodyInstance.GetComponent<AnimationControllerScript>();
        }
        BodyInstance.SetActive(true);
        BodyInstance.transform.localPosition = Vector3.zero;
        enemy.animator = Animator;
        enemy.animController = AnimatorController;
    }

    public void Destroy()
    {
        BodyInstance.transform.SetParent(null);
        BodyInstance.SetActive(false);
    }
}
