using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public float MaxTime = 100f;
    private float timer;
    private void OnEnable()
    {
        timer = 0f;
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > MaxTime)
        {
            Destroy(this.gameObject);
        }
    }
    public void IsInactive(bool isInactive)
    {
        this.enabled = !isInactive;
    }
}