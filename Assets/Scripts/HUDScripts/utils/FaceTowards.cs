using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FaceTowards : MonoBehaviour
{
	/*
	//per casa: il laser va solo dal target al gun(fix scala z), add quad (sun glow) al target, segue la cam, mettere prefab axis per studiare le inquadrature
	//con uno script input,far scorrere la camera tra i punti scelti con slerp
	//cntrl-shift-f : scorciatoia camera
	//bit.ly/OK_TURRET_END_00_AJ
	*/

	public Transform target, TurretBase, gun,laser;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{

		//base gun turning
		if (target.position.y > gun.transform.position.y) {
			gun.LookAt (target);
			laser.gameObject.SetActive (true);
		}
		else
			laser.gameObject.SetActive (false);

		//turret base turning
		Quaternion baseY = Quaternion.LookRotation ((target.position - TurretBase.position).normalized);
		TurretBase.transform.localRotation = new Quaternion (TurretBase.transform.localRotation.x,
			baseY.y,
			TurretBase.transform.localRotation.z,
			baseY.w);

	}
}
//	Vector3 targetPos;
//target.position - transform.position


//	transform.rotation= Quaternion.LookRotation (target.position - transform.position, Vector3.up);


//usare lookat? lookrotation (base)
// lookrotation mantenendo v3.up

//	Vector3	dirToTarget = transform.position - target.position;

//transform.rotation= Quaternion.LookRotation(dirToTarget,Vector3.up)  //insta rotation


/*
 * //WAYS TO FIND A GMOBJ//
 * 
 * gmobj.find (name)
 * 
 * gmobj.getcomponent(s)(in children / parent)<T>()
 * gmobj.findgmobjwith Tag (tagname)
 * gmobj.findobjofType<t>()
 * 
 * gmobj.addcomponent<T>();
 * 
		// GMOBJ CHILD ACCESS//

 * 
 * traform.childcount/parent/ root (root of all parents)
 * transform.GetChild(index)
 * transform.setparent(transfrm)
 * 
 * 
*/