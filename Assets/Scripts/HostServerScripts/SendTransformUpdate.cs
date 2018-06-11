using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SendTransformUpdate : MonoBehaviour
{
    public float PacketSendInterval = 0.1f;
    float timer;

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = PacketSendInterval;
            Client.SendTransformToHost(transform, EP2PSend.k_EP2PSendUnreliable);
        }
    }
}
