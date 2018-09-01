using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableShadows : MonoBehaviour
{
    MeshRenderer mesh;
    SkinnedMeshRenderer skinMesh;

    // Use this for initialization
    void Start()
    {
        skinMesh = GetComponent<SkinnedMeshRenderer>();
        if (skinMesh == null)
            mesh = GetComponent<MeshRenderer>();
    }

    public void Disable()
    {
        if (skinMesh != null)
            skinMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        else if (mesh != null)
            mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void Enable()
    {
        if (skinMesh != null)
            skinMesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        else if (mesh != null)
            mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
}
