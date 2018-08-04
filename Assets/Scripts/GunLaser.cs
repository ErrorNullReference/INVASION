using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class GunLaser : MonoBehaviour
{
    public Transform Muzzle;
    public LayerMaskHolder Mask;
    public float MaxDistance = 100, Width = 0.05f;
    public Color Color;
    Transform cube;
    GameObject pivot;
    Ray ray;
    RaycastHit hit;

    // Use this for initialization
    void Start()
    {
        pivot = new GameObject();
        pivot.transform.SetParent(Muzzle);
        pivot.transform.localPosition = Vector3.zero;

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        cube.SetParent(pivot.transform);
        cube.localPosition = new Vector3(0, 0, 0.5f);
        Destroy(cube.GetComponent<Collider>());
        cube.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
        cube.GetComponent<Renderer>().material.SetColor("_Color", Color);

        ray = new Ray();
    }
	
    // Update is called once per frame
    void Update()
    {
        ray.origin = Muzzle.position;
        ray.direction = Muzzle.forward;

        float magnitude = MaxDistance;
        if (Physics.Raycast(ray, out hit, MaxDistance, Mask.LayerMask.value))
            magnitude = (ray.origin - hit.point).magnitude;

        pivot.transform.localScale = new Vector3(Width, Width, magnitude);
    }
}
