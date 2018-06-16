using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
public enum PowerUpType
{
    Health = 0,
    Energy = 1
}

public class Timer
{
    public float MaxTime { get { return timeLimit; } }
    public bool IsActive;

    public float currentTime;
    private float timeLimit;

    public Timer(float timeLimit)
    {
        this.timeLimit = timeLimit;
    }

    public void Update(float deltaTime)
    {
        currentTime += deltaTime;
    }

    public bool IsOver()
    {
        return currentTime > timeLimit;
    }
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType Type;
    [NonSerialized]
    public SOPool Pool;

    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private int minValue;
    [SerializeField]
    private int maxValue;

    private int value;
    private Timer timer;

    private void OnEnable()
    {
        value = UnityEngine.Random.Range(minValue, maxValue + 1);
        if (timer == null)
            timer = new Timer(lifeTime);
        timer.currentTime = 0;
    }

    private void Update()
    {
        timer.Update(Time.deltaTime);
        if (timer.IsOver())
        {
            timer.currentTime = 0f;
            //Recycle
            Pool.Recycle(this.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        Player p = collision.gameObject.GetComponent<Player>();
        if (!p)
            return;

        if (Type == PowerUpType.Health)
        {
            Debug.Log("Before: " + p.Life);
            p.Life += value;
        }
        Debug.Log("After : " + p.Life);

        //Manager call the recycle of this istance maybe
        Pool.Recycle(this.gameObject);
    }
}
