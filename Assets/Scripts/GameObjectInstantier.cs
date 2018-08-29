using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectInstantier : MonoBehaviour
{
    public GameObject[] Objects;

    // Use this for initialization
    public void Init()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            Instantiate(Objects[i], this.transform);
        }
    }
}
