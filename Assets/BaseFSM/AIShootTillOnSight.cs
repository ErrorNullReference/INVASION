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
    private LayerMaskHolder layerToShootAt;
    [SerializeField]
    private float maxViewDistance;
    [SerializeField]
    private float damage;

    [SerializeField]
    private float minCooldownBetweenShoots;
    [SerializeField]
    private float maxCooldownBetweenShoots;
    private float currentCooldownBetweenShoots;

    [SerializeField]
    private Transform muzzle;
    private Transform targetToShot;

    public EnemyShootSync shootSync;

    [SerializeField]
    private AIBehaviour next;

    protected override void Awake()
    {
        base.Awake();
        shootSync = this.GetComponent<EnemyShootSync>();
        ResetShootCooldown();
    }

    private void Update()
    {
        this.transform.LookAt(targetToShot.position);

        currentCooldownBetweenShoots -= Time.deltaTime;

        if(CheckIfStillOnSight() && currentCooldownBetweenShoots <= 0)
            Shot();
    }

    private void Shot()
    {
        RaycastHit hit;
        if(Physics.Raycast(this.muzzle.position, this.transform.forward, out hit, maxViewDistance, layerToShootAt))
        {
            Player hitPlayer = hit.collider.GetComponent<Player>();
            if (hitPlayer && !hitPlayer.Dead)
                hitPlayer.DecreaseLife(this.damage);
        }

        ResetShootCooldown();
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
        ResetShootCooldown();
    }

    private void ResetShootCooldown()
    {
        this.currentCooldownBetweenShoots = UnityEngine.Random.Range(minCooldownBetweenShoots, maxCooldownBetweenShoots);
    }

    public override void OnStateExit()
    {
        targetToShot = null;
    }
}
