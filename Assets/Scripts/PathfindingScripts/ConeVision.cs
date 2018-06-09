using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOPRO;
public class ConeVision : MonoBehaviour
{
    public SOEvGameObject OnPlayerSight;

    public LayerMaskHolder Mask;
    List<Player> possibleTargets;
    [SerializeField]
    private bool workAsClient;

    [SerializeField]
    [Range(1f, 360f)]
    private float fov;

    [SerializeField]
    private float maxViewDistance;

    private Player[] connectedPlayers;

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

    private void Start()
    {
        if (connectedPlayers == null)
        {
            connectedPlayers = FindObjectsOfType<Player>();
            foreach (ConeVision component in FindObjectsOfType<ConeVision>())
                component.SetPlayers(connectedPlayers);
        }
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
        if (connectedPlayers == null)
            return;

        possibleTargets.Clear();
        for (int i = 0; i < connectedPlayers.Length; i++)
        {
            float distanceToPlayer = Vector3.Distance(this.transform.position, connectedPlayers[i].gameObject.transform.position);
            if (distanceToPlayer > maxViewDistance)
                continue;

            Vector3 directionToPlayer = connectedPlayers[i].gameObject.transform.position - this.transform.position;
            float angleToPlayer = Vector3.Angle(this.transform.forward, directionToPlayer);
            if (angleToPlayer > fov * 0.5f)
                continue;

            RaycastHit hit;

            Vector3 raycastPositionStart = transform.position + new Vector3(0f, 0.5f, 0f);
            Debug.DrawRay(raycastPositionStart, directionToPlayer, Color.magenta);
            if (Physics.Raycast(raycastPositionStart, directionToPlayer, out hit, maxViewDistance, Mask))
            {
                if (hit.collider.gameObject.GetComponentInParent<Player>() != null)
                {
                    possibleTargets.Add(connectedPlayers[i]);
                }
            }
        }

        if (possibleTargets.Count == 0)
            return;

        target = possibleTargets[Random.Range(0, possibleTargets.Count)].gameObject;
        //movementManager.SetDestination(possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)]);
    }

    public void SetPlayers(Player[] connectedPlayers)
    {
        if (connectedPlayers != null)
            this.connectedPlayers = connectedPlayers;
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
