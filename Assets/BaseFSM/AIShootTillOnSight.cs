using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class AIShootTillOnSight : AIBehaviour
{
    [SerializeField]
    private LayerMaskHolder layerToLookInto;
    [SerializeField]
    private float maxViewDistance;

    private Transform targetToShot;

    public EnemyShootSync shootSync;

    [SerializeField]
    private AIBehaviour next;

    protected override void Awake()
    {
        base.Awake();
        shootSync = this.GetComponent<EnemyShootSync>();
    }

    private void Update()
    {
        this.transform.LookAt(targetToShot.position);

        if(CheckIfStillOnSight())
            Shot();
    }

    private void Shot()
    {
        //Debug.Log("BANG BANG");
        if (shootSync != null)
            shootSync.SendShotCall();
    }

    private bool CheckIfStillOnSight()
    {
        RaycastHit hit;
        if (!Physics.Raycast(this.transform.position + new Vector3(0, 0.5f, 0), this.transform.forward, out hit, maxViewDistance, layerToLookInto))
        {
            owner.SwitchState(next);
            return false;
        }

        if(hit.collider.transform == targetToShot)
            return true;
        else
        {
            owner.SwitchState(next);
            return false;
        }
    }

    public override void OnStateEnter()
    {
        AIGoToTargetUntilOnSight previousState = owner.PreviousState as AIGoToTargetUntilOnSight;
        if (previousState == null)
        {
            owner.SwitchState(next);
            return;
        }

        targetToShot = previousState.Target;
    }

    public override void OnStateExit()
    {
        targetToShot = null;
    }
}
