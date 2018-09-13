using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using Steamworks;
using UnityEngine.AI;
using GENUtility;
using SOPRO;

[CreateAssetMenu (menuName = "Network/EnemySpawner")]
public class EnemySpawner : Factory<byte>
{
	public static int PreInstantiedCount = 20;

	public SODictionaryTransformContainer netEntities;
	public GameObject PoolRoot;

	public SOVariableInt EnemiesCount;

	public NavMeshAreaMaskHolder Mask;

	public SOListGenericContainer EnemyStatsPool;

	public GameObject EnemyTemplate;

	public EnemyInitializer[] EnemyInitializers;

	private Transform poolRoot;

	private Pool enemyPool;

	public void Init ()
	{
		Client.AddCommand (PacketType.EnemyDeath, OnEnemyDeath);
		Client.AddCommand (PacketType.EnemyDown, OnEnemyDown);
		Client.AddCommand (PacketType.EnemySpawn, InstantiateEnemy);
		EnemiesCount.Value = 0;

		if (!poolRoot && PoolRoot) {
			poolRoot = GameObject.Instantiate (PoolRoot).transform;
			poolRoot.name = "Enemies Root";
			poolRoot.gameObject.AddComponent<ObjectsRegister> ().Obj = this;
		}
		enemyPool = new Pool (EnemyTemplate, poolRoot, PreInstantiedCount);

		for (int i = 0; i < EnemyInitializers.Length; i++)
			EnemyInitializers [i].InitInstances ();
	}

	protected override byte ExtractIdentifier (GameObject obj, int i)
	{
		return (byte)i;
		//return (byte)obj.GetComponent<Enemy>().Type.Value;
	}

	private void InstantiateEnemy (byte[] data, uint length, CSteamID senderId)
	{
		byte type = data [0];
		int id = ByteManipulator.ReadInt32 (data, 1);

		float positionX = ByteManipulator.ReadSingle (data, 5);
		float positionY = ByteManipulator.ReadSingle (data, 9);
		float positionZ = ByteManipulator.ReadSingle (data, 13);

		Vector3 position = new Vector3 (positionX, positionY, positionZ);

		GameObject go = enemyPool.Get ();
		go.transform.position = position;
		go.transform.rotation = Quaternion.identity;

		Enemy enemy = go.GetComponent<Enemy> ();
		enemy.NetObj.SetNetworkId (id);
		enemy.Initializer = EnemyInitializers [(int)type];
		enemy.Init ((EnemyStats)EnemyStatsPool.Elements [0]); //[type]

		NavMeshHit hit;
		if (NavMesh.SamplePosition (position, out hit, 1f, Mask))
			go.GetComponent<NavMeshAgent> ().Warp (hit.position);

		go.SetActive (true);

		EnemiesCount.Value++;
	}

	/*//InstantiateEnemy will be called when command EnemySpawn is received from host
	private void InstantiateEnemy (byte[] data, uint length, CSteamID senderId)
	{
		if (!poolRoot && PoolRoot) {
			poolRoot = GameObject.Instantiate (PoolRoot).transform;
			poolRoot.name = "Enemies Root";
			poolRoot.gameObject.AddComponent<ObjectsRegister> ().Obj = this;
		}

		byte type = data [0];
		int id = ByteManipulator.ReadInt32 (data, 1);

		float positionX = ByteManipulator.ReadSingle (data, 5);
		float positionY = ByteManipulator.ReadSingle (data, 9);
		float positionZ = ByteManipulator.ReadSingle (data, 13);

		Vector3 position = new Vector3 (positionX, positionY, positionZ);

		SOPool pool = organizedPools [(int)EnemyType.Normal];
		//SOPool pool = organizedPools[type];

		bool parented;
		int nullObjsRemovedFromPool;
		GameObject go = pool.DirectGet (poolRoot, position, Quaternion.identity, out nullObjsRemovedFromPool, out parented);

		Enemy enemy = go.GetComponent<Enemy> ();
		enemy.Pool = pool;
		enemy.NetObj.SetNetworkId (id);
		enemy.Initializer = EnemyInitializers [(int)type];
		enemy.Init ((EnemyStats)EnemyStatsPool.Elements [0]); //[type]

		NavMeshHit hit;
		if (NavMesh.SamplePosition (position, out hit, 1f, Mask))
			go.GetComponent<NavMeshAgent> ().Warp (hit.position);

		go.SetActive (true);

		EnemiesCount.Value++;
	}*/

	//OnEnemyDeath will be called when command EnemyDeath is received from host
	private void OnEnemyDeath (byte[] data, uint length, CSteamID senderId)
	{
		int Id = ByteManipulator.ReadInt32 (data, 0);

		if (Id >= netEntities.Elements.Count)
			return;

		if (!netEntities.Elements.ContainsKey (Id))
			return;

		Enemy obj = netEntities [Id].GetComponent<Enemy> ();

		if (!obj)
			throw new NullReferenceException ("NetId does not correspond to an enemy");

		//obj.Pool.Recycle (obj.gameObject);
		enemyPool.Recycle(obj.gameObject);

		EnemiesCount.Value--;
	}

	private void OnEnemyDown (byte[] data, uint length, CSteamID senderId)
	{
		int Id = ByteManipulator.ReadInt32 (data, 0);

		if (Id >= netEntities.Elements.Count)
			return;

		if (!netEntities.Elements.ContainsKey (Id))
			return;

		Enemy obj = netEntities [Id].GetComponent<Enemy> ();

		if (!obj)
			throw new NullReferenceException ("NetId does not correspond to an enemy");

		obj.Down ();
	}

	class Pool
	{
		Dictionary<int, GameObject> objs;
		GameObject template;
		Transform root;
		int index;

		public Pool (GameObject template, Transform root, int preInstanceCount = 0)
		{
			objs = new Dictionary<int, GameObject> (PreInstantiedCount);
			this.template = template;
			this.root = root;
			index = 0;

			for (int i = 0; i < preInstanceCount; i++) {
				objs.Add (i, Instantiate (template, root));
				objs[i].SetActive(false);
			}
		}

		public GameObject Get ()
		{
			for (int i = index; i < objs.Count; i++) {
				if (objs [i].activeSelf == false) {
					objs [i].SetActive (true);
					index = i;
					return objs [i];
				}
			}

			for (int i = 0; i < index; i++) {
				if (objs [i].activeSelf == false) {
					objs [i].SetActive (true);
					index = i;
					return objs [index];
				}
			}

			GameObject o = Instantiate (template, root);
			index = objs.Count;
			objs.Add (index, o);
			return o;
		}

		public void Recycle (GameObject item)
		{
			for (int i = 0; i < objs.Count; i++)
				if (objs [i] == item)
					objs [i].SetActive (false);
		}

		public void RecycleAll ()
		{
			for (int i = 0; i < objs.Count; i++)
				objs [i].SetActive (false);
		}
	}
}
