using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingOrientation
{
down,
up,
forward,
back,
left,
right
}

public class CameraFacingBillboard : MonoBehaviour
{
	public Camera cam;

	public FacingOrientation orientation;
	Vector3 fromRotation, toRotation;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		toRotation = cam.transform.position;
		switch (orientation)
		{
		case FacingOrientation.forward:
			fromRotation = Vector3.forward;
			break;

		case FacingOrientation.back:
			fromRotation = Vector3.back;
			break;
		}


		Vector3 dirToCam = cam.transform.position - transform.position;
		//	transform.rotation = Quaternion.FromToRotation (fromRotation, dirToCam); //move the obj forward to the camera (rotation gets wonky)
		//transform.rotation= Quaternion.LookRotation(dirToCam,Vector3.up); proper insta rotation
		transform.rotation= Quaternion.RotateTowards(transform.rotation,   //delayed rotation
			Quaternion.FromToRotation(fromRotation,dirToCam),
			20*Time.deltaTime);

		//full code: bit.ly/LIVE_BILLBOARD_END_00_SD

		/*turret needs to turn around its y axis  (turretdirection.cs)
		 * gun obj should rotate around x axis
		 * 
		 * 
		 * */
	}
}
