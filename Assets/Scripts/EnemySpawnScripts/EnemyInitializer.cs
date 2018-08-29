using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnemyInitializer : ScriptableObject
{
    public GameObject Body;

    public void Init(Enemy enemy, Transform root)
    {
        GameObject b = Instantiate(Body, root);
        b.transform.localPosition = Vector3.zero;
        enemy.animator = b.GetComponent<Animator>();
        enemy.animController = b.GetComponent<AnimationControllerScript>();
    }
}
