using UnityEngine;
using SOPRO;
public class MouseShooter : MonoBehaviour
{
    public SOPool Pool;
    public KeyCode ShootKey = KeyCode.Mouse0;
    public Camera Camera;
    void Update()
    {
        if (Input.GetKeyDown(ShootKey))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            int nullObjsRemovedFromPool;
            Pool.Get(null, ray.origin, Quaternion.LookRotation(ray.direction, this.transform.up), out nullObjsRemovedFromPool);
        }
    }
}