using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyDrop : MonoBehaviour {

    public int EnergyAmount;
    private float timer;

    private void Start()
    {
        EnergyAmount = UnityEngine.Random.Range(1, 4);
        float energyScale = (1f / 3f) * EnergyAmount;
        gameObject.transform.localScale = new Vector3(energyScale, energyScale, energyScale);
        timer = 10f;
    }

   	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if(timer<=0)
        {
            Destroy(this.gameObject);
        }
	}
}
