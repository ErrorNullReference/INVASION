using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplePlayerControllerAnimator : MonoBehaviour
{
	public Animator animator;
	public string paramNameTranslate, paramNameRotate;
	public bool sendTranslateParam;
	public bool rotateRoot;
	private float slowModifierKey;

	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public KeyCode triggerKey;

	void FixedUpdate()
	{
		var x = Input.GetAxis("Horizontal");
		var z = Input.GetAxis("Vertical");



		if (rotateRoot)
			transform.Rotate(0, x * 3f, 0);

		float slowModifierVal = 0.4f;
		/*if (Input.GetKey(slowModifierKey))
            slowModifierVal = 0.5f;*/

		if (sendTranslateParam && z != 0)
			animator.SetFloat(paramNameTranslate, z * slowModifierVal);
		//Debug.Log("Horizontal " + x);
	}

	void Update()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit, 100))
		{
			transform.LookAt(hit.point);
		}

		if (Input.GetKeyDown(triggerKey))
        {
            Fire();
        }
	}

	void Fire()
	{
		var bullet = Instantiate(
			bulletPrefab,
			bulletSpawn.position,
			bulletSpawn.rotation);

		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;

		Destroy(bullet, 2.0f);
	}
}
