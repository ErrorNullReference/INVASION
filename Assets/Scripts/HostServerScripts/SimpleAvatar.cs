using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAvatar : MonoBehaviour
{
    //Used by other classes.
    public ShootSystem ShootSystem { get; private set; }

    public PlayerAnimatorController AnimatorController { get; private set; }

    //Used by other classes.
    public User UserInfo { get; set; }

    public Player Player { get; set; }

    Prediction predition;
    Vector3 startPos, endPos, speed;
    Quaternion startRot, endRot;
    float interpolationTime, time, frac;
    bool updateTransform;

    void Start()
    {
        ShootSystem = GetComponent<ShootSystem>();     
        AnimatorController = GetComponentInChildren<PlayerAnimatorController>();
        Player = GetComponent<Player>();

        startPos = endPos = transform.position;
        startRot = endRot = transform.rotation;

        predition = new Prediction(startPos, startRot);
    }

    //PREDICTION
    public void SetTransform(Vector3 position, Quaternion rotation)
    {
        startPos = transform.position;
        startRot = transform.rotation;

        if (predition != null)
            interpolationTime = predition.Predict(position, rotation, out endPos, out endRot, out speed);
        time = 0;

        updateTransform = true;
    }

    //CHANGED
    //Ricreato l'update con l'if, il lerp deve essere chiamato in update per poter funzionare.
    private void Update()
    {
        if (!updateTransform)
            return;
        
        time += Time.deltaTime;
        frac = time / interpolationTime;
        if (frac > 1)
        {
            frac = 1;
            updateTransform = false;
        }
        transform.position = Vector3.Lerp(startPos, endPos, frac);
        transform.rotation = Quaternion.Slerp(startRot, endRot, frac);
    }

    public void Shoot()
    {
        ShootSystem.Shoot();
        AnimatorController.Shoot();
    }
}
