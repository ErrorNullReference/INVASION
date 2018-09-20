using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInitDebug : MonoBehaviour
{
    public PowerUpsMgr Mgr;

    // Use this for initialization
    void Start()
    {
        Mgr.Init();
    }
}
