using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;

public class AIShootTillOnSight : AIBehaviour
{
    [SerializeField]
    private LayerMaskHolder targetLayerMask;
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
    private Player targetToShot;

    public EnemyShootSync shootSync;

    [SerializeField]
    private AIBehaviour targetLost;

    protected override void Awake()
    {
        base.Awake();
        shootSync = this.GetComponent<EnemyShootSync>();
        ResetShootCooldown();

        Client.OnUserDisconnected += (id) =>
        {
            targetToShot = null;
        };
    }

    private void Update()
    {
        if (targetToShot != null)
            this.transform.LookAt(targetToShot.transform.position);

        currentCooldownBetweenShoots -= Time.deltaTime;

        if (currentCooldownBetweenShoots <= 0)
            Shot();
    }

    private void Shot()
    {
        if (targetToShot == null || targetToShot.Dead)
        {
            owner.SwitchState(targetLost);
            return;
        }

        RaycastHit hit;
        Vector3 currentPos = transform.position;
        Vector3 dir = (targetToShot.transform.position - currentPos).normalized;
        Transform shot = null;


        if (Physics.Raycast(currentPos + new Vector3(0, 0.5f, 0), dir, out hit, maxViewDistance, targetLayerMask))
            shot = hit.collider.transform.root;

        if (!shot || shot != targetToShot.transform)
        {
            owner.SwitchState(targetLost);
            return;
        }

        targetToShot.DecreaseLife(this.damage);
        transform.LookAt(shot, Vector3.up);

        ResetShootCooldown();
        if (shootSync != null)
            shootSync.SendShotCall();
    }

    public override void OnStateEnter()
    {
        AIGoToTargetUntilOnSight previousState = owner.PreviousState as AIGoToTargetUntilOnSight;
        if (previousState == null)
        {
            owner.SwitchState(targetLost);
            return;
        }

        targetToShot = previousState.Target.GetComponent<Player>();
        if (!targetToShot || targetToShot.Dead)
        {
            owner.SwitchState(targetLost);
            return;
        }

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
