using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColor : MonoBehaviour
{
    public SkinnedMeshRenderer Mesh;
    public Color[] Colors;
    Enemy enemy;
    bool Updated;

    void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        Updated = false;
    }

    void Update()
    {
        if (!Updated)
        {
            if (enemy != null)
            {
                int t = (int)((EnemyStats)enemy.Stats).Type;
                if (t < Colors.Length)
                    Mesh.material.color = Colors[t];
            }
            Updated = true;
        }
    }
}
