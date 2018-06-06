using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIShootTillOnSight : AIBehaviour
{
    [SerializeField]
    private LayerMask layerToLookInto;
    [SerializeField]
    private float maxViewDistance;

    private GameObject targetToShot;

    public UnityEvent OnTargetLossOfSight;

    public EnemyShootSync shootSync;

    private void Awake()
    {
        shootSync = this.GetComponent<EnemyShootSync>();
    }

    public override void AIUpdate()
    {
        this.transform.LookAt(targetToShot.transform.position);

        if(CheckIfStillOnSight())
            Shot();
    }

    private void Shot()
    {
        Debug.Log("BANG BANG");
        if (shootSync != null)
            shootSync.SendShotCall();
    }

    private bool CheckIfStillOnSight()
    {
        RaycastHit hit;
        if (!Physics.Raycast(this.transform.position + new Vector3(0, 0.5f, 0), this.transform.forward, out hit, maxViewDistance, layerToLookInto))
        {
            OnTargetLossOfSight.Invoke();
            return false;
        }
        return true;
    }

    public override void OnStateEnter()
    {
        AIGoToTargetUntilOnSight previousState = owner.PreviousState as AIGoToTargetUntilOnSight;
        if (previousState == null)
        {
            OnTargetLossOfSight.Invoke();
            return;
        }

        targetToShot = previousState.Target;
    }

    public override void OnStateExit()
    {
        targetToShot = null;
    }
}
