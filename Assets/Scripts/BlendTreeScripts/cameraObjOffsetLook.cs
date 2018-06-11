using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  cameraObjOffsetLook: MonoBehaviour {
	public Transform parentObj;
	public Vector3 offset;

	void Update () {
		transform.position = parentObj.position + offset;
		transform.LookAt (parentObj.position, Vector3.up);
	}
    public void doit()
    {

    }
}
