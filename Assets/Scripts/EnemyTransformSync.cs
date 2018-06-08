using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTransformSync : MonoBehaviour
{
    private float time, interpolationTime, frac;
    private WaitForSeconds waitForSecond;
    private Prediction prediction;
    private Vector3 startPos, endPos, speed;
    private Quaternion startRot, endRot;
    private AnimationControllerScript animController;

    private void Awake()
    {
        waitForSecond = new WaitForSeconds(0.1f);
        animController = GetComponent<AnimationControllerScript>();
    }
    private void OnEnable()
    {
        if (Client.IsHost)
            StartCoroutine(SendTransform());

        startPos = endPos = transform.position;
        startRot = endRot = transform.rotation;
        prediction = new Prediction(startPos, startRot);
    }
    private void OnDisable()
    {
        if (Client.IsHost)
            StopCoroutine(SendTransform());
    }
    private IEnumerator SendTransform()
    {
        while (true)
        {
            byte[] payload = new byte[(sizeof(float) * 7) + 1];
            int index = 0;

            byte[] id = new byte[] { (byte)this.GetComponent<GameNetworkObject>().NetworkId };

            Array.Copy(id, 0, payload, index, 1);
            index++;
            Array.Copy(BitConverter.GetBytes(this.transform.position.x), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.position.y), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.position.z), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.x), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.y), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.z), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.w), 0, payload, index, sizeof(float));

            Client.SendPacketToInGameUsers(payload, PacketType.EnemyTransform, Client.MyID, Steamworks.EP2PSend.k_EP2PSendUnreliable, false);
            yield return waitForSecond;
        }
    }

	void Update () 
    {
        if (!Client.IsHost)
        {
            time += Time.deltaTime;
            frac = time / interpolationTime;
            if (frac > 1)
                frac = 1;
            transform.position = Vector3.Lerp(startPos, endPos, frac);
            transform.rotation = Quaternion.Slerp(startRot, endRot, frac);
            return;
        }
        
    }
    public void ReceiveTransform(Vector3 pos, Quaternion rot) //USED BY MOVEMENTMANAGER
    {
       // Debug.Log("Enemy: " + this.GetComponent<GameNetworkObject>().NetworkId + "Position is: " + pos);

        startPos = transform.position;
        startRot = transform.rotation;
        interpolationTime = prediction.Predict(pos, rot, out endPos, out endRot, out speed);
        Vector3 normalizedSpeed = speed.normalized;
        animController.Animation(normalizedSpeed.x, normalizedSpeed.z);
        time = 0;
    }
}
