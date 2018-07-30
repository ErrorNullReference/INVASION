using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSubMeshes : MonoBehaviour
{
    public Material Material;
    public int SubMeshToModify, SubMeshToCopy;
    public bool SetSubMeshNumber;
    public int SubMeshNumber;
    SkinnedMeshRenderer renderer;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<SkinnedMeshRenderer>();
        if (renderer == null)
            return;

        Mesh m = renderer.sharedMesh;
        int[] vs = m.GetTriangles(SubMeshToCopy < m.subMeshCount ? SubMeshToCopy : 0);

        if (SetSubMeshNumber)
            m.subMeshCount = SubMeshNumber;
        if (SubMeshToModify >= m.subMeshCount)
            SubMeshToModify = m.subMeshCount - 1;
        
        m.SetTriangles(vs, SubMeshToModify);

        Material[] ms = renderer.sharedMaterials;

        if (ms.Length <= SubMeshToModify)
        {
            Material[] newMs = new Material[ms.Length + (SubMeshToModify - ms.Length + 1)];
            System.Array.Copy(ms, 0, newMs, 0, ms.Length);
            newMs[SubMeshToModify] = Material;
            renderer.sharedMaterials = newMs;
        }
        else
        {
            ms[SubMeshToModify] = Material;
            renderer.sharedMaterials = ms;
        }
    }
}
