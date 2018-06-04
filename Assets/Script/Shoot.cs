using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

    public ParticleSystem part0;
    public ParticleSystem part1;
	public ParticleSystem part2;
	public ParticleSystem part3;
	public int ParticlesToEmit0,ParticlesToEmit1,ParticlesToEmit2;
	public int ParticlesToEmit3;
	public bool Continuois;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!Continuois) {
        	if (Input.GetKeyDown(KeyCode.Mouse0))
        	{
        	    if (part0 != null)
        	    {
        	   		 part0.Emit(ParticlesToEmit0);
        	    }
        	    if (part1 != null)
        	    {
					part1.Emit(ParticlesToEmit1);
        	    }
				if (part2 != null)
				{
					part2.Emit(ParticlesToEmit2);
				}
				if (part3 != null)
				{
					part3.Emit(ParticlesToEmit3);
				}
        	}
		} else
		{
			if (Input.GetKey(KeyCode.Mouse0))
			{
				if (part0 != null)
				{
					part0.Emit(ParticlesToEmit0);

				}
				if (part1 != null)
				{
					part1.Emit(ParticlesToEmit1);

				}
				if (part2 != null)
				{
					part2.Emit(ParticlesToEmit2);

				}
				if (part3 != null)
				{
					part3.Emit(ParticlesToEmit3);
				}
			}
		}
	}
}
