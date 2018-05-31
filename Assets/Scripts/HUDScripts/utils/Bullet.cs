using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed;
	public float lifeTime;
	private float timeLeft;

	// Update is called once per frame
	void Update () {
		timeLeft += Time.deltaTime;
		if (lifeTime > timeLeft)
			transform.position += transform.forward * speed * Time.deltaTime;
		else
			GameObject.Destroy (this.gameObject);
	}
}
