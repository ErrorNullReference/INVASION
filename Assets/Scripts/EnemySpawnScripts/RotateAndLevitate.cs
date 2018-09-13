using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndLevitate : MonoBehaviour
{
    public Vector3 Rotation;
    public float LevitationForce, LevitationDuration;
    Vector3 startPos, endPos, initPos;
    float timer, frac;

    void Start()
    {
        initPos = transform.position;
        Init();
    }

    void OnValidate()
    {
        Init();
    }

    void Init()
    {
        startPos = initPos - new Vector3(0, LevitationForce, 0);
        endPos = initPos + new Vector3(0, LevitationForce, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        frac = timer / LevitationDuration;

        transform.position = Vector3.Lerp(startPos, endPos, frac);
        transform.Rotate(Rotation);

        if (frac >= 1)
        {
            timer = 0;
            Vector3 pos = startPos;
            startPos = endPos;
            endPos = pos;
        }
    }
}
