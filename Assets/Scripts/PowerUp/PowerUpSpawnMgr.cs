using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
using Steamworks;
using GENUtility;

public class PowerUpSpawnMgr : MonoBehaviour
{
    public SOListVector3Container Container;
    List<Vector3> free;
    static PowerUpSpawnMgr instance;
    public AudioClip[] Clips;
    public SoundEmitter Emitter;

    void Start()
    {
        instance = this;

        free = new List<Vector3>();
        for (int i = 0; i < Container.Elements.Count; i++)
            free.Add(Container.Elements[i]);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while (Client.AddCommand(PacketType.PowerUpSound, MakeSound))
        {
            yield return null;
        }
    }

    public static Vector3 GetPosition()
    {
        if (instance.free.Count == 0)
            return instance.Container.Elements[0];

        int r = Random.Range(0, instance.free.Count);
        Vector3 v = instance.free[r];
        instance.free.RemoveAt(r);
        return v;
    }

    public static void FreePosition(Vector3 pos)
    {
        for (int i = 0; i < instance.Container.Elements.Count; i++)
        {
            if (instance.Container.Elements[i] == pos)
                instance.free.Add(pos);
        }
    }

    void MakeSound(byte[] data, uint length, CSteamID sender)
    {
        int type = ByteManipulator.ReadInt32(data, 0);
        if (type < Clips.Length)
        {
            Emitter.SetClip(Clips[type]);
            Emitter.EmitSound();
        }
    }
}
