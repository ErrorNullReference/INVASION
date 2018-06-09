using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOPRO;
public class ShootOnSight : MonoBehaviour
{
    public float GunRange;

    public SOEvVoid OnAbleToShot;

    private LayerMaskHolder mask;

    //private void Start()
    //{
    //    mask = LayerMask.GetMask("Default", "Player");
    //}

    void Update ()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.gameObject.transform.position + this.transform.forward * 0.1f, this.transform.forward, out hit, GunRange, mask))
        {
            if (hit.collider.gameObject.GetComponent<Player>())
            {
                OnAbleToShot.Raise();
                Debug.Log(hit.collider.gameObject.name);
            }
        }
	}
}
