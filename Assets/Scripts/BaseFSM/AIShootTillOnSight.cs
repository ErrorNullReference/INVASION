using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class AIShootTillOnSight : AIBehaviour
{
    [SerializeField]
    private SOListPlayerContainer playersAlive;
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

        if (CheckIfStillOnSight() && currentCooldownBetweenShoots <= 0)
            Shot();
    }

    private void Shot()
    {
        RaycastHit hit;
        for (int i = 0; i < playersAlive.Elements.Count; i++)
        {
            Player player = playersAlive[i];
            if (player.PlayerCollider.Raycast(new Ray(this.muzzle.position, this.transform.forward), out hit, maxViewDistance))
            {
                player.DecreaseLife(this.damage);
                break;
            }
        }

        ResetShootCooldown();
        if (shootSync != null)
            shootSync.SendShotCall();
    }

    private bool CheckIfStillOnSight()
    {
        RaycastHit hit;
        bool lostSight = true;
        for (int i = 0; i < playersAlive.Elements.Count; i++)
        {
            Player player = playersAlive[i];

            if (player.transform != targetToShot)
                continue;

            lostSight = !player.PlayerCollider.Raycast(new Ray(this.transform.position + new Vector3(0, 0.5f, 0), this.transform.forward), out hit, maxViewDistance);

            break;
        }

        if (lostSight)
            owner.SwitchState(next);

        return !lostSight;
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
