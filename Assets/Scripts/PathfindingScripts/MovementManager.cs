using System;
using Steamworks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Collections;

[Serializable]
public class UnityEventPassingGameObject : UnityEvent<GameObject> { }

[RequireComponent(typeof(NavMeshAgent))]
public class MovementManager : MonoBehaviour
{
    [SerializeField]
    private float tolerance;
    [SerializeField]
    private float cooldownBetweenRecalculations;
    private float currentCooldownLeft;
    private NavMeshAgent agent;
    private Vector3 oldDestination;
    private Vector3 nextDestination;
    private Quaternion oldQuatenion;
    private Quaternion nextQuaternion;
    private AnimationControllerScript animController;
    private delegate void NetworkState();

    private NetworkState networkState;
    public UnityEvent OnPathReached;
    public UnityEvent OnPathStarted;
    private WaitForSeconds waitForSecond = new WaitForSeconds(0.1f);

    private Prediction prediction;
    private float time, interpolationTime, frac;
    private Vector3 startPos, endPos, speed;
    private Quaternion startRot, endRot;

    private void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
        currentCooldownLeft = cooldownBetweenRecalculations;
        animController = GetComponent<AnimationControllerScript>();
    }

    private void OnEnable()
    {
        this.agent.enabled = true;
        //this.agent.isStopped = true;
        if (Client.IsHost)
            StartCoroutine(SendTransform());

        //oldDestination = nextDestination = transform.position;
        //TO DO: set start rotation
        startPos = endPos = transform.position;
        startRot = endRot = transform.rotation;
        prediction = new Prediction(startPos, startRot);
    }

    private void OnDisable()
    {
        this.agent.enabled = false;
        if (Client.IsHost)
            StopCoroutine(SendTransform());
    }

    private void Start()
    {
        if (Client.IsHost)
            networkState = HostState;
        else
            networkState = ClientState;
    }

    private void Update()
    {
        networkState.Invoke();
    }

    private void HostState()
    {
        //Sets the last received destination only if cooldownBetweenRecalculation has reached 0 and if the new point is farther than the tolerance.
        currentCooldownLeft -= Time.deltaTime;
        if (currentCooldownLeft <= 0 && (oldDestination - nextDestination).magnitude > tolerance)
        {
            this.agent.isStopped = false;
            currentCooldownLeft = cooldownBetweenRecalculations;
            agent.SetDestination(nextDestination);
            oldDestination = nextDestination;
            OnPathStarted.Invoke();
        }

        //Check if agent reached the destination, if yes calls the OnPathReached Event and then this won't be called again unless a new destination is set.
        if (!this.agent.isStopped && this.agent.hasPath && (this.transform.position - this.agent.destination).magnitude < tolerance)
        {
            this.agent.isStopped = true;
            OnPathReached.Invoke();
        }
        Vector3 direction = this.agent.velocity.normalized;        
        animController.Animation(direction.x, direction.z);
        //SendTransform();
    }

    private IEnumerator SendTransform()
    {
        while (true)
        {
            byte[] payload = new byte[(sizeof(float) * 7) + 1];
            int index = 0;

            byte[] id = new byte[] { (byte)this.GetComponent<GameNetworkObject>().NetworkId };

            Array.Copy(id, 0, payload, index, 1);
            index++;
            Array.Copy(BitConverter.GetBytes(this.transform.position.x), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.position.y), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.position.z), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.x), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.y), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.z), 0, payload, index, sizeof(float));
            index += sizeof(float);
            Array.Copy(BitConverter.GetBytes(this.transform.rotation.w), 0, payload, index, sizeof(float));

            Client.SendPacketToInGameUsers(payload, PacketType.EnemyTransform, Client.MyID, Steamworks.EP2PSend.k_EP2PSendUnreliable, false);
            yield return waitForSecond;
        }
    }

    private void ClientState()
    {
        /*if (nextDestination != oldDestination)
            this.transform.position = nextDestination;
        if (nextQuaternion != oldQuatenion)
            this.transform.rotation = nextQuaternion;*/

        time += Time.deltaTime;
        frac = time / interpolationTime;
        if (frac > 1)
            frac = 1;
        transform.position = Vector3.Lerp(startPos, endPos, frac);
        transform.rotation = Quaternion.Slerp(startRot, endRot, frac);
          
        //transform.position = Vector3.Lerp(oldDestination, nextDestination, frac);
    }

    public void ReceiveTransform(Vector3 pos, Quaternion rot)
    {
        Debug.Log("Enemy: " + this.GetComponent<GameNetworkObject>().NetworkId + "Position is: " + pos);

        //oldDestination = nextDestination;
        //oldQuatenion = nextQuaternion;

        //nextDestination = pos;
        //nextQuaternion = rot;

        startPos = transform.position;
        startRot = transform.rotation;
        interpolationTime = prediction.Predict(pos, rot, out endPos, out endRot, out speed);
        Vector3 normalizedSpeed = speed.normalized;
        animController.Animation(normalizedSpeed.x, normalizedSpeed.z);
        time = 0;
    }

    public void SetDestination(Vector3 position)
    {
        nextDestination = position;
    }

    public void SetDestination(GameObject target)
    {
        SetDestination(target.transform.position);
    }
}