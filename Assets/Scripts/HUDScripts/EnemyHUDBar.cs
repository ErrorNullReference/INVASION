using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHUDBar : MonoBehaviour {

	public GameObject bar;
	public Gradient healthColorRange;
	public float energy;
//	public float Energy{ get {return energy;} set {energy = value;} }
	public float maxEnergy;

	void OnCollisionEnter(Collision col){
		Bullet playerBullet = col.gameObject.GetComponent<Bullet> ();
		if (playerBullet != null)
			energy -= 5;

		if (energy <= 0) {
			GameObject.Destroy (this.gameObject);	//die
		}
			
	}

	// Update is called once per frame
	void Update () {
		bar.GetComponent<MeshRenderer>().material.color = healthColorRange.Evaluate (energy / maxEnergy);
	}
}
