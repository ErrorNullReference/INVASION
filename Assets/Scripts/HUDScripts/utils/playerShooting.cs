using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerShooting : MonoBehaviour {

	public GameObject bulletPrefab;
	public KeyCode shootKey;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (shootKey))
			GameObject.Instantiate (bulletPrefab, transform.position, Quaternion.identity);
	}
}
