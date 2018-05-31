using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCameraLerp : MonoBehaviour
{

    public List<Transform> players;
    public Vector3 cameraOffset;
    private Vector3 center;
    private Vector3 velocity;
    public float Smoothness;
    public float minZoom;
    public float maxZoom;
    public float zoomVelocity;
    


    private void LateUpdate()
    {
        if (players.Count == 0)
            return;

            center = GetCenter();
            Vector3 newCameraPos = center + cameraOffset;
            this.transform.position = Vector3.SmoothDamp(transform.position, newCameraPos, ref velocity, Smoothness);

       
            AutoZoom();
        
    }

    private Vector3 GetCenter()
    {
        if (players.Count == 1)
            return players[0].position;

        Bounds bounds = new Bounds(players[0].position, Vector3.zero);

        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        return bounds.center;
    }

    private float GetMaxDistance()
    {
        Bounds bounds = new Bounds(players[0].position, Vector3.zero);

        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        return bounds.size.x;
    }

    private void AutoZoom()
    {
            float autoZoom = Mathf.Lerp(maxZoom, minZoom, GetMaxDistance());
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, autoZoom, zoomVelocity * Time.deltaTime);

    }


}
