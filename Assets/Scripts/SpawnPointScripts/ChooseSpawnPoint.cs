using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class ChooseSpawnPoint : MonoBehaviour
{
    public bool IsDead = false;
    public int PosIndex;
    void Start()
    {
        transform.position = PointManager.Instance.nodes[PosIndex];
    }

    void Update()
    {
        if (IsDead == true)
        {
            transform.position = PointManager.Instance.nodes[UnityEngine.Random.Range(0, PointManager.Instance.nodes.Count)];
            IsDead = false;
        }
    }
}
