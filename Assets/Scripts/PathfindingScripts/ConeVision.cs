using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOPRO;
public class ConeVision : MonoBehaviour
{
    public BaseSOEvGameObject OnPlayerSight;

    List<Player> possibleTargets;
    [SerializeField]
    private bool workAsClient;

    [SerializeField]
    [Range(1f, 360f)]
    private float fov;

    [SerializeField]
    private float maxViewDistance;

    [SerializeField]
    private SOListPlayerContainer players;

    private GameObject target;

    private bool isLooking;

    private void Awake()
    {
        if (!workAsClient && !Client.IsHost)
            this.enabled = false;
        possibleTargets = new List<Player>();
    }

    private void OnEnable()
    {
        isLooking = true;
    }

    private void Update()
    {
        target = null;

        if (isLooking)
            CheckPlayerInVision();

        if (target != null)
            OnPlayerSight.Raise(target);
    }

    private void CheckPlayerInVision()
    {
        int length = players.Elements.Count;
        Vector3 pos = transform.position;

        possibleTargets.Clear();

        for (int i = 0; i < length; i++)
        {
            Player p = players[i];
            Vector3 playerPos = p.transform.position;
            float distanceToPlayer = Vector3.Distance(pos, playerPos);
            if (distanceToPlayer > maxViewDistance)
                continue;

            Vector3 directionToPlayer = playerPos - pos;
            float angleToPlayer = Vector3.Angle(this.transform.forward, directionToPlayer);
            if (angleToPlayer > fov * 0.5f)
                continue;

            RaycastHit hit;

            Vector3 raycastPositionStart = pos + new Vector3(0f, 0.5f, 0f);
            Debug.DrawRay(raycastPositionStart, directionToPlayer, Color.magenta);
            if (p.PlayerCollider.Raycast(new Ray(raycastPositionStart, directionToPlayer), out hit, maxViewDistance))
            {
                possibleTargets.Add(p);
            }
        }

        if (possibleTargets.Count == 0)
            return;

        target = possibleTargets[Random.Range(0, possibleTargets.Count)].gameObject;
        //movementManager.SetDestination(possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)]);
    }

    public void StopLooking()
    {
        isLooking = false;
    }

    public void StartLooking()
    {
        isLooking = true;
    }
}
