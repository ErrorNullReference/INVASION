using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SOPRO;
using GENUtility;

public class ShootSystem : MonoBehaviour
{
	public Gun gun;
	public Muzzle muzzle;
	[SerializeField]
	private bool EnableRaycastControl;
	[SerializeField]
	private LayerMaskHolder controlMask;
	private Ray controlRay;
	private float recoilTime;
	private static readonly byte[] emptyArray = new byte[0];
	bool menuOpen, active;

	SimpleAvatar avatar;

	private void Awake ()
	{
		recoilTime = 0;
		avatar = GetComponent<SimpleAvatar> ();
		if (EnableRaycastControl)
			controlRay = new Ray ();
	}

	void Start ()
	{
		MenuEvents.OnMenuOpen += () => menuOpen = true;
		MenuEvents.OnMenuClose += () => menuOpen = false;

		Enable ();
	}

	void OnDestroy ()
	{

		MenuEvents.OnMenuOpen -= () => menuOpen = true;
		MenuEvents.OnMenuClose -= () => menuOpen = false;
	}

	void CallShoot (uint shootIndex)
	{
		if (EnableRaycastControl)
		{
			controlRay.origin = transform.position + new Vector3(0,1.5f,0);
			Vector3 dir = gun.Projectile.position - controlRay.origin;
			controlRay.direction = dir.normalized;
			if (Physics.Raycast (controlRay, dir.magnitude, controlMask)) 
			{
				gun.EmitSound (shootIndex);
				return;
			}
		}

		Shoot (shootIndex);
		SendShootToAll (shootIndex);
	}

	public void Shoot (uint shootIndex)
	{
		gun.Shoot (shootIndex);
	}

	void SendShootToAll (uint index)
	{
		if (Client.IsHost)
			Client.SendPacketToInGameUsers (new byte[]{ (byte)index }, 0, 1, PacketType.PlayerShoot, Steamworks.EP2PSend.k_EP2PSendReliable, false);
		else
			Client.SendPacketToHost (new byte[]{ (byte)index }, 0, 1, PacketType.PlayerShootServer, Steamworks.EP2PSend.k_EP2PSendReliable);
	}

	public void SendHitMessage (int id, float damage)
	{
		byte[] data = ArrayPool<byte>.Get (16);
		ByteManipulator.Write (data, 0, id);
		ByteManipulator.Write (data, 4, damage);
		ByteManipulator.Write (data, 8, (ulong)avatar.UserInfo.SteamID);

		if (Client.IsHost)
			Client.SendPacketToInGameUsers (data, 0, data.Length, PacketType.ShootHit, Steamworks.EP2PSend.k_EP2PSendReliable, true);
		else
			Client.SendPacketToHost (data, 0, data.Length, PacketType.ShootHitServer, Steamworks.EP2PSend.k_EP2PSendReliable);

		ArrayPool<byte>.Recycle (data);
	}

	void Update ()
	{
		if (menuOpen || !active)
			return;

		//different beheaviour with number
		recoilTime -= Time.deltaTime;
		if (Input.GetButtonDown ("Fire1") && recoilTime <= 0) {
			CallShoot (0);
			recoilTime = gun.values.Rateo;
		} else if (Input.GetButtonDown ("Fire2") && recoilTime <= 0) {
			CallShoot (1);
			recoilTime = gun.values.Rateo;
		}
	}

	public void Enable ()
	{
		active = true;
	}

	public void Disable ()
	{
		active = false;
	}
}