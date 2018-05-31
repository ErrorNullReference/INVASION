using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameNetworkObject : MonoBehaviour {

    [SerializeField]
    private int networkId;
    public int NetworkId
    {
        get
        {
            return networkId;
        }
        set
        {
            networkId = value;
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
