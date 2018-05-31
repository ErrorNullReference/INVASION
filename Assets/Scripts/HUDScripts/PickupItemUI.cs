using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemUI : MonoBehaviour {

	public Camera cam;
	// Update is called once per frame
	void Update () {
		Vector3  dirToCam= cam.transform.position - transform.position;
		transform.rotation =	Quaternion.FromToRotation (Vector3.back, dirToCam);
	}
}
