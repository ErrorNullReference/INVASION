using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GENUtility;

public class NetObjTransformSync : MonoBehaviour
{
	public float AcceptanceTreshold;

	private static readonly BytePacket payload = new BytePacket ((sizeof(float) * 7) + sizeof(int));
	private WaitForSeconds waitForSecond;

	private float time, interpolationTime, frac;
	private Prediction prediction;
	private Vector3 startPos, endPos, speed;
	private Quaternion startRot, endRot;

	private AnimationControllerScript animController;
	private GameNetworkObject gnObject;
	private Transform myTransform;

	private void Awake ()
	{
		waitForSecond = new WaitForSeconds (0.1f);
		animController = GetComponent<AnimationControllerScript> ();
	}

	public void Enable ()
	{
		if (!myTransform) {
			gnObject = GetComponent<GameNetworkObject> ();
			myTransform = transform;
		}
		if (Client.IsHost)
			StartCoroutine (SendTransform ());

		startPos = endPos = transform.position;
		startRot = endRot = transform.rotation;
		if (prediction == null)
			prediction = new Prediction (startPos, startRot);
		else
			prediction.Reset (startPos, startRot);
	}

	public void Disable ()
	{
		if (Client.IsHost)
			StopCoroutine (SendTransform ());
	}

	private IEnumerator SendTransform ()
	{
		while (true) {
			payload.CurrentLength = 0;
			payload.CurrentSeek = 0;

			payload.Write (gnObject.NetworkId); 

			Vector3 pos = myTransform.position;
			Quaternion rot = myTransform.rotation;

			payload.Write (pos.x);
			payload.Write (pos.y);
			payload.Write (pos.z);

			payload.Write (rot.x);
			payload.Write (rot.y);
			payload.Write (rot.z);
			payload.Write (rot.w);

			Client.SendPacketToInGameUsers (payload.Data, 0, payload.MaxCapacity, PacketType.NetObjTransform, Client.MyID, Steamworks.EP2PSend.k_EP2PSendUnreliable, false);
			yield return waitForSecond;
		}
	}

	void Update ()
	{
		if (!Client.IsHost) {
			time += Time.deltaTime;
			frac = time / interpolationTime;
			if (frac > 1)
				frac = 1;
			myTransform.position = Vector3.Lerp (startPos, endPos, frac);
			myTransform.rotation = Quaternion.Slerp (startRot, endRot, frac);

			if (Mathf.Approximately (frac, 1))
				animController.Animation (0, 0, 0);
			return;
		}

	}

	public void ReceiveTransform (Vector3 pos, Quaternion rot) //USED BY MOVEMENTMANAGER
	{
		// Debug.Log("Enemy: " + this.GetComponent<GameNetworkObject>().NetworkId + "Position is: " + pos);

		startPos = myTransform.position;
		startRot = myTransform.rotation;
		interpolationTime = prediction.Predict (pos, rot, out endPos, out endRot, out speed);

		if (speed.magnitude / Time.deltaTime >= AcceptanceTreshold)
			animController.Animation (transform.forward, speed, speed.magnitude / Time.deltaTime);
		time = 0;
	}
}
