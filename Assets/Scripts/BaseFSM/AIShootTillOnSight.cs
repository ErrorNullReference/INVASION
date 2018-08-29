using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SOPRO;

public class AIShootTillOnSight : AIBehaviour
{
    [SerializeField]
    private LayerMaskHolder targetLayerMask;
    [SerializeField]
    private float maxViewDistance;
    [SerializeField]
    private float midDistance;
    [SerializeField]
    private float minDistance;
    [SerializeField]
    [Range(1, 360)]
    private int fov;
    [SerializeField]
    private float minCooldownBetweenShoots;
    [SerializeField]
    private float maxCooldownBetweenShoots;
    private float currentCooldownBetweenShoots;
    [SerializeField]
    private float cooldownBeforeRecalculation;
    private float currentCooldownLeftBeforeRecalculation;

    private Player targetToShot;

    public EnemyShootSync shootSync;
    Gun gun;

    [SerializeField]
    private AIBehaviour targetLost;
    [SerializeField]
    private AIBehaviour goToPlayer;

    private NavMeshHit navMeshHit;
    private NavMeshAgent agent;
    private Enemy enemy;

    private AnimationControllerScript animController { get { return enemy.animController; } }

    protected override void Awake()
    {
        base.Awake();
        shootSync = this.GetComponent<EnemyShootSync>();
        agent = this.GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();

        if (shootSync == null)
            shootSync = GetComponent<EnemyShootSync>();
        if (targetLost == null)
            targetLost = GetComponent<AIAggroPlayers>();
        if (goToPlayer == null)
            goToPlayer = GetComponent<AIGoToTargetUntilOnSight>();

        ResetShootCooldown();

        Client.OnUserDisconnected += (id) =>
        {
            targetToShot = null;
        };

        gun = GetComponentInChildren<Gun>();
    }

    private void Update()
    {
        Move();

        if (targetToShot != null)
            this.transform.LookAt(targetToShot.transform.position);

        currentCooldownBetweenShoots -= Time.deltaTime;

        if (currentCooldownBetweenShoots <= 0)
            Shot();
    }

    private void Move()
    {
        currentCooldownLeftBeforeRecalculation -= Time.deltaTime;
        if (targetToShot != null && currentCooldownLeftBeforeRecalculation <= 0)
        {
            int index = CheckIfTargetInVision();
            if (index == -1)
                this.agent.isStopped = true;
            else
            {
                this.agent.isStopped = false;
                if (index == 0)
                    this.agent.SetDestination(targetToShot.transform.position);
                else
                {
                    Vector3 destination = (transform.position - targetToShot.transform.position).normalized * midDistance;
                    if (!this.agent.SetDestination(destination))
                    {
                        if (NavMesh.SamplePosition((targetToShot.transform.position - transform.position).normalized * (minDistance * 1.5f), out navMeshHit, minDistance * 0.5f, NavMesh.AllAreas))
                            this.agent.SetDestination(navMeshHit.position);
                        else
                            this.agent.isStopped = true;
                    }
                }
            }
        }

        Vector3 vel = this.agent.velocity.normalized;
        animController.Animation(vel.x, vel.z);
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

        targetToShot.DecreaseLife(owner.enemy != null ? ((EnemyStats)owner.enemy.Stats).Damage : 1);
        transform.LookAt(shot, Vector3.up);

        ResetShootCooldown();
        if (shootSync != null)
            shootSync.SendShotCall();

        if (gun != null)
            gun.Shoot();
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

        agent.isStopped = false;
    }

    private void ResetShootCooldown()
    {
        this.currentCooldownBetweenShoots = UnityEngine.Random.Range(minCooldownBetweenShoots, maxCooldownBetweenShoots);
    }

    public override void OnStateExit()
    {
        targetToShot = null;
        agent.isStopped = true;
    }

    private int CheckIfTargetInVision()
    {
        if (targetToShot == null)
        {
            owner.SwitchState(targetLost);
            return -1;
        }

        Vector3 targetPos = targetToShot.transform.position;
        Vector3 pos = transform.position;

        float distanceToPlayer = Vector3.Distance(pos, targetPos);
        if (distanceToPlayer > maxViewDistance)
        {
            owner.SwitchState(goToPlayer);
            return -1;
        }

        Vector3 directionToPlayer = (targetPos - pos).normalized;
        float angleToPlayer = Vector3.Angle(this.transform.forward, directionToPlayer);
        if (angleToPlayer > fov * 0.5f)
            return 0;

        RaycastHit hit;

        Vector3 raycastPositionStart = pos + new Vector3(0f, 0.5f, 0f);

        if (!targetToShot || !targetToShot.gameObject.activeSelf)
            owner.SwitchState(targetLost);
        else if (Physics.Raycast(raycastPositionStart, directionToPlayer, out hit, maxViewDistance, targetLayerMask) && hit.collider.transform.root == targetToShot.transform)
        {
            if (distanceToPlayer > midDistance)
                return 0;
            else if (distanceToPlayer < minDistance)
                return 1;
        }

        return -1;
    }
}
