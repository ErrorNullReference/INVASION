using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInitDebug : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        if (Client.instance != null)
        {
            Client.instance.DebugInit();    

            Client.SendPacketToHost(new byte[0], 0, 0, PacketType.GameEntered, Steamworks.EP2PSend.k_EP2PSendReliable);
        }
        else
            StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while (Client.instance == null)
            yield return null;

        Client.instance.DebugInit();    

        Client.SendPacketToHost(new byte[0], 0, 0, PacketType.GameEntered, Steamworks.EP2PSend.k_EP2PSendReliable);
    }
}
