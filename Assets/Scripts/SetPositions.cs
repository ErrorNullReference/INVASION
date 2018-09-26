using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class SetPositions : MonoBehaviour
{
    public SOListVector3Container PositionsToFill;
    public bool ClearFirst = true, DrawGizmos = true;
	public Color Color = Color.red;

    void Start()
    {
        if (ClearFirst)
            PositionsToFill.Elements.Clear();

        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int i = 0; i < childs.Length; i++)
        {
            PositionsToFill.Elements.Add(childs[i].position);
        }
        Destroy(this);
    }

    void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;
        
		Gizmos.color = Color;
        Transform[] childs = GetComponentsInChildren<Transform>();
        for (int i = 1; i < childs.Length; i++)
        {
            Gizmos.DrawSphere(childs[i].position, 1);
        }
    }
}
