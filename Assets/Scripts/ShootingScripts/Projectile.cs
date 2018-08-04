using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class Projectile : MonoBehaviour
{
    public float Speed;
    private float time;
    public float Timer;
    public ParticleSystem[] PS;
    public Transform ChildRoot;

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
            Deactivate();
        }
    }

    private void MoveForward()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Deactivate();  
    }

    void Deactivate()
    {
        for (int i = 0; i < PS.Length; i++)
        {
            ParticleSystem.MainModule m = PS[i].main;
            m.loop = false;
        }
        ChildRoot.SetParent(null);
        Destroy(this.gameObject);
    }
}
