using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootOnSight : MonoBehaviour
{
    public float GunRange;

    public UnityEvent OnAbleToShot;

    private int mask;

    private void Start()
    {
        mask = LayerMask.GetMask("Default", "Player");
    }

    void Update ()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.gameObject.transform.position + this.transform.forward * 0.1f, this.transform.forward, out hit, GunRange, mask))
        {
            if (hit.collider.gameObject.GetComponent<Player>())
            {
                OnAbleToShot.Invoke();
                Debug.Log(hit.collider.gameObject.name);
            }
        }
	}
}
