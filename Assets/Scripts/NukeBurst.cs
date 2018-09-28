using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using Steamworks;

public class NukeBurst : MonoBehaviour
{
    private PostProcessingBehaviour post;
    [SerializeField]
    private bool Burst;
    public BloomModel.Settings UpdatedSetting;
    public AnimationCurve IntesnityCurve;
    public AnimationCurve DeltaTimeCurve;
    public EnemySpawner Spawner;

    private float AnimationTime;
    private bool update, init;
	private SoundEmitter emitter;

    // Use this for initialization
    void Start()
    {
        post = GetComponent<PostProcessingBehaviour>();
		emitter = GetComponent<SoundEmitter> ();
        init = false;
    }

    void ManageNuke(byte[] Data, uint length, CSteamID ID)
    {
        update = true;
        Burst = true;

		if (Client.IsHost) 
		{
			foreach (Enemy item in Spawner.enemyPool.objs.Values)
			{
				if (item.Active)
				{
					byte[] data = new byte[sizeof(int) + sizeof(float) + sizeof(ulong)];
					byte[] id = System.BitConverter.GetBytes (item.GetComponent<GameNetworkObject> ().NetworkId);
					byte[] damage = System.BitConverter.GetBytes (item.MaxLife);
					System.Array.Copy (id, 0, data, 0, id.Length);
					System.Array.Copy (damage, 0, data, id.Length, damage.Length);
					System.Array.Copy (Data, 0, data, id.Length + damage.Length, Data.Length);

					Client.SendPacketToInGameUsers (data, 0, data.Length, PacketType.ShootHit, EP2PSend.k_EP2PSendReliable, true);
				}
			}
		}

		if (emitter != null)
			emitter.EmitSound ();
    }

    void Reset()
    {
        UpdatedSetting.bloom.intensity = IntesnityCurve.Evaluate(0);
        UpdatedSetting.bloom.antiFlicker = true;
        post.profile.bloom.settings = UpdatedSetting;
    }

    void OnDestroy()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            Client.AddCommand(PacketType.Nuke, ManageNuke);
            init = true;
        }

        if (!update)
            return;

        if (Burst)
        {
            if (AnimationTime <= 1)
            {
                AnimationTime += Time.deltaTime;
            }
            if (AnimationTime >= 1)
            {
                Burst = false;
            }
        }
        Time.timeScale = DeltaTimeCurve.Evaluate(AnimationTime);
        if (!Burst)
        {
            if (AnimationTime >= 0)
            {
                AnimationTime -= Time.deltaTime;
            }
            else if (AnimationTime < 0)
            {
                AnimationTime = 0;
                update = false;
            }
        }
        UpdatedSetting.bloom.intensity = IntesnityCurve.Evaluate(AnimationTime);
        UpdatedSetting.bloom.antiFlicker = true;
        post.profile.bloom.settings = UpdatedSetting;
    }
}
