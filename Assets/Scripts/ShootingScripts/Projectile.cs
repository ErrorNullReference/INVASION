using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class Projectile : MonoBehaviour
{
    public float Speed;
    private float time;
    public float Timer;

    private void OnEnable()
    {
        time = Timer;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        MoveForward();
        if (time <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveForward()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Destroy(this.gameObject);
    }
}
