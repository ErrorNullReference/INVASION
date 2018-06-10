using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class simplePlayerControllerAnimator : MonoBehaviour
{
    public Camera Main;
	public Animator animator;
	public AnimatorPropertyHolder paramNameTranslate, paramNameRotate;
	public bool sendTranslateParam;
	public bool rotateRoot;
	private float slowModifierKey;

	public SOPool bulletPool;
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
			animator.SetFloat((int)paramNameTranslate, z * slowModifierVal);
		//Debug.Log("Horizontal " + x);
	}

	void Update()
	{
		RaycastHit hit;
		Ray ray = Main.ScreenPointToRay(Input.mousePosition);

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
        GameObject bullet = bulletPool.Get(null, bulletSpawn.position, bulletSpawn.rotation);

		bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;
	}
}
