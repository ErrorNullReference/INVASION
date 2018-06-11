using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class FixedCameraLerp : MonoBehaviour
{

    public SOListPlayerContainer players;
    public Vector3 cameraOffset;
    public Camera Main;
    private Vector3 center;
    private Vector3 velocity;
    public float Smoothness;
    public float minZoom;
    public float maxZoom;
    public float zoomVelocity;



    private void LateUpdate()
    {
        if (players.Elements.Count == 0)
            return;

        center = GetCenter();
        Vector3 newCameraPos = center + cameraOffset;
        this.transform.position = Vector3.SmoothDamp(transform.position, newCameraPos, ref velocity, Smoothness);


        AutoZoom();

    }

    private Vector3 GetCenter()
    {
        int length = players.Elements.Count;
        if (length == 0)
            return new Vector3();

        Vector3 firstPos = players[0].transform.position;

        if (length == 1)
            return firstPos;

        Bounds bounds = new Bounds(firstPos, Vector3.zero);

        for (int i = 0; i < length; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.center;
    }

    private float GetMaxDistance()
    {
        Bounds bounds = new Bounds(players[0].transform.position, Vector3.zero);
        int length = players.Elements.Count;
        for (int i = 0; i < length; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.size.x;
    }

    private void AutoZoom()
    {
        float autoZoom = Mathf.Lerp(maxZoom, minZoom, GetMaxDistance());
        Main.fieldOfView = Mathf.Lerp(Main.fieldOfView, autoZoom, zoomVelocity * Time.deltaTime);

    }


}
