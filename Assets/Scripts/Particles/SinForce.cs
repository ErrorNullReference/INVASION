using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinForce : MonoBehaviour
{
    public ParticleSystem PS;
    public float Mult, TimeMult;
    ParticleSystem.Particle[] particles;

    // Use this for initialization
    void Start()
    {
        if (PS == null)
            PS = GetComponent<ParticleSystem>();
    }
	
    // Update is called once per frame
    void Update()
    {
        if (PS == null)
            return;

        particles = new ParticleSystem.Particle[PS.particleCount];
        int count = PS.GetParticles(particles);

        Vector3 dir = new Vector3(Mathf.Cos(Time.time * TimeMult) * Mult, Mathf.Sin(Time.time * TimeMult) * Mult, Mult);
        for (int i = 0; i < count; i++)
        {
            particles[i].position += dir * Time.deltaTime;
        }

        PS.SetParticles(particles, count);
    }
}
