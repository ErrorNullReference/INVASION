using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(SoundEmitter))]
public class FootstepsSound : MonoBehaviour
{
    public AudioClip[] Cemento, Piastrelle, Metallo;
    public float FootstepInterval = 0.2f, StepTreshold = 0.1f;
    public SoundEmitter emitter;
    Rigidbody bodY;
    float t;
    Vector3 oldPos;
    NavMeshHit navHit;
    int cemento, metallo, piastrelle;

    // Use this for initialization
    void Start()
    {
        bodY = GetComponent<Rigidbody>();	

        cemento = 1 << NavMesh.GetAreaFromName("Cemento");
        metallo = 1 << NavMesh.GetAreaFromName("Metallo");
        piastrelle = 1 << NavMesh.GetAreaFromName("Piastrelle");
    }

    void OnEnable()
    {
        oldPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - oldPos).sqrMagnitude > StepTreshold)
        {
            t += Time.deltaTime;
            if (t > FootstepInterval)
            {
                t = 0;
                if (emitter != null)
                {
                    if (NavMesh.SamplePosition(transform.position, out navHit, 0.5f, 255))
                    {
                        if ((navHit.mask & cemento) != 0)
                            emitter.Clips = Cemento;
                        else if ((navHit.mask & metallo) != 0)
                            emitter.Clips = Metallo;
                        else if ((navHit.mask & piastrelle) != 0)
                            emitter.Clips = Piastrelle;
                        else
                        {
                            oldPos = transform.position;
                            return;
                        }
                        emitter.EmitSound();
                    }
                }
            }
        }
        else
            t = 0;

        oldPos = transform.position;
    }
}
