using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPrefabSpawner : MonoBehaviour
{

	public GameObject prefab;
	public Transform prefabPosition;

	public void ShowAvatar()
	{
		GameObject instance = Instantiate(prefab);
		instance.transform.position = prefabPosition.position;
	}
}
                    