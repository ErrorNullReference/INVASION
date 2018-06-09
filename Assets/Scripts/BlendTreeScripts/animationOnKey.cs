using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
//Same as animationOnTriggers.cs, but with Keys
public class animationOnKey : MonoBehaviour
{
	public Animator animator;
	public AnimatorPropertyHolder paramName;
	public bool setBoolOnlyWhenPressed, setBoolPressOneTime;
	public bool newBoolVal;
	public bool setTrigger;

	public KeyCode triggerKey;


	void FixedUpdate()
	{
		var x = Input.GetAxis("Horizontal");//TODO: why is this in fixedupdate?
		var z = Input.GetAxis("Vertical");

	}
	void Update()
	{
		if (Input.GetKey(triggerKey))
		{
			if (setTrigger)
				animator.SetTrigger((int)paramName);
		}


		if (setBoolOnlyWhenPressed)
		{
			if (Input.GetKey(triggerKey))
				animator.SetBool((int)paramName, newBoolVal);
			else
				animator.SetBool((int)paramName, !newBoolVal);
		}

		if (setBoolPressOneTime)
		{
			if (Input.GetKeyDown(triggerKey))
				animator.SetBool((int)paramName, newBoolVal);
			else
				animator.SetBool((int)paramName, !newBoolVal);
		}

	}

}