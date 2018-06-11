using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostEnemyDestroyer : MonoBehaviour {

    public static HostEnemyDestroyer Instance;
    public static List<Enemy> EnemyToRecycle;
    public static List<Enemy> EnemyToRecycleToAdd;
    public static List<Enemy> EnemyToRecycleToRemove;
    // Use this for initialization
    void Start () {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        else Instance = this;
        if (!Client.IsHost)
        {
            this.enabled = false;
            return;
        }
        EnemyToRecycle = new List<Enemy>();
        EnemyToRecycleToAdd = new List<Enemy>();
        EnemyToRecycleToRemove = new List<Enemy>();
    }
	
	// Update is called once per frame
	void Update () {      
       foreach(Enemy e in EnemyToRecycleToRemove)
        {
            EnemyToRecycle.Remove(e);
        }
        foreach (Enemy e in EnemyToRecycleToAdd)
        {
            EnemyToRecycle.Add(e);
        }
        EnemyToRecycleToRemove.Clear();
        EnemyToRecycleToAdd.Clear();
        foreach (Enemy e in EnemyToRecycle)
        {
            if (e.Destroy || e.Recycling)
            {
                HostEnemyDestroyer.Instance.EnemyDeath(e);
            }
        }
    }

    private void EnemyDeath(Enemy enemy)
    {
        if (HostEnemySpawner.Instance.enemiesCount <HostEnemySpawner.MAX_NUM_ENEMIES)
        {
            if (enemy.Destroy)
            {
                Client.SendPacketToInGameUsers(new byte[] { (byte)enemy.NetworkId }, PacketType.EnemyDeath, Steamworks.EP2PSend.k_EP2PSendReliable);
                enemy.Destroy = false;
            }
            enemy.randomSpawnTimer -= Time.deltaTime;
            if (enemy.randomSpawnTimer <= 0)
            {
                HostEnemySpawner.Instance.InstantiateEnemy(enemy.NetworkId);
                enemy.randomSpawnTimer = UnityEngine.Random.Range(0f, 5.0f);
                enemy.Recycling = false;
                EnemyToRecycleToRemove.Add(enemy);
            }
        }
    }

    
}
