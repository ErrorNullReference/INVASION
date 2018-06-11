using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseSpawnPoint : MonoBehaviour
{
    private int count;
    public bool IsDead = false;
    public int PosIndex;
	
	void Start ()
    {
        transform.position = PointManager.Instance.nodes[PosIndex];
        count = PointManager.Instance.nodes.Count;
    }
	
	void Update ()
    {
        if (IsDead == true)
        {
            transform.position = PointManager.Instance.nodes[UnityEngine.Random.Range(0, count)];
            IsDead = false;
            return;
        }
	}
}
