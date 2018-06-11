using UnityEngine;
public class MouseShooter : MonoBehaviour
{
    public GameObject Prefab;
    public KeyCode ShootKey = KeyCode.Mouse0;
    public Camera Camera;
    void Update()
    {
        if (Input.GetKeyDown(ShootKey))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
            GameObject.Instantiate(Prefab, ray.origin, Quaternion.LookRotation(ray.direction, this.transform.up));
        }
    }
}