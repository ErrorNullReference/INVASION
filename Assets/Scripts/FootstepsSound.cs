using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SoundEmitter))]
public class FootstepsSound : MonoBehaviour
{
	public float FootstepInterval = 0.2f, StepTreshold = 0.1f;
	Rigidbody bodY;
	SoundEmitter emitter;
	float t;
	Vector3 oldPos;

	// Use this for initialization
	void Start ()
	{
		bodY = GetComponent<Rigidbody> ();	
		emitter = GetComponent<SoundEmitter> ();	
	}

	void OnEnable ()
	{
		oldPos = transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		if ((transform.position - oldPos).sqrMagnitude > StepTreshold)
		{
			t += Time.deltaTime;
			if (t > FootstepInterval)
			{
				t = 0;
				emitter.EmitSound ();
			}
		} else
			t = 0;

		oldPos = transform.position;
	}
}
