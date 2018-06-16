using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0,360)));
	}
}
