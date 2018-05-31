using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Same as animationOnTriggers.cs, but with Keys
public class animationOnKey : MonoBehaviour
{
	public Animator animator;
	public string paramName;
	public bool setBoolOnlyWhenPressed, setBoolPressOneTime;
	public bool newBoolVal;
	public bool setTrigger;

	public KeyCode triggerKey;


	void FixedUpdate()
	{
		var x = Input.GetAxis("Horizontal");
		var z = Input.GetAxis("Vertical");

	}
	void Update()
	{
		if (Input.GetKey(triggerKey))
		{
			if (setTrigger)
				animator.SetTrigger(paramName);
		}


		if (setBoolOnlyWhenPressed)
		{
			if (Input.GetKey(triggerKey))
				animator.SetBool(paramName, newBoolVal);
			else
				animator.SetBool(paramName, !newBoolVal);
		}

		if (setBoolPressOneTime)
		{
			if (Input.GetKeyDown(triggerKey))
				animator.SetBool(paramName, newBoolVal);
			else
				animator.SetBool(paramName, !newBoolVal);
		}

	}

}