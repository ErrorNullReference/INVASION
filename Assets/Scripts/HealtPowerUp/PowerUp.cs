using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Healt = 0,
    Energy = 1
}

public class Timer
{
    public float MaxTime { get { return timeLimit; } }
    public bool IsActive { get; set; }

    public float currentTime;
    private float timeLimit;

    public Timer(float timeLimit)
    {
        this.timeLimit = timeLimit;
    }

    public void Update()
    {
        currentTime += Time.deltaTime;
    }

    public bool IsOver()
    {
        return currentTime > timeLimit;
    }

    public void Reset()
    {
        currentTime = 0f;
    }
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType Type;

    [SerializeField]
    private float lifeTime;
    [SerializeField]
    private int minValue;
    [SerializeField]
    private int maxValue;

    private int value;
    private Timer timer;

    private void Start()
    {
        value = Random.Range(minValue,maxValue);
        timer = new Timer(lifeTime);
    }

    private void Update()
    {
        timer.Update();
        if (timer.IsOver())
        {
            timer.Reset();
            //Recycle
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null && Type == PowerUpType.Healt)
        {
            Debug.Log("Before: " + collision.gameObject.GetComponent<Player>().Life);
            collision.gameObject.GetComponent<Player>().Life += value;
        }
        Debug.Log("After : " + collision.gameObject.GetComponent<Player>().Life);

        //Manager call the recycle of this istance maybe
        Destroy(gameObject);
    }
}
