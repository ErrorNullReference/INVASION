using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOPRO;
public class ShootOnSight : MonoBehaviour
{
    public float GunRange;

    public BaseSOEvVoid OnAbleToShot;

    [SerializeField]
    private SOListPlayerContainer players;

    void Update()
    {
        RaycastHit hit;

        Transform t = transform;
        Vector3 pos = t.position;
        Vector3 forward = t.forward;

        int length = players.Elements.Count;

        for (int i = 0; i < length; i++)
        {
            Player p = players[i];
            if (p.PlayerCollider.Raycast(new Ray(pos + forward * 0.1f, forward), out hit, GunRange))
            {
                OnAbleToShot.Raise();
            }
        }
    }
}