﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _AIProjectile : MonoBehaviour
{
    public bool IsMoving;
    public float LifeTime;
    public float Speed;

    void Start()
    {
        IsMoving = true;
        Destroy(this.gameObject, LifeTime);
    }

    void FixedUpdate()
    {
        if (IsMoving)
        {
            transform.position += transform.forward * Speed * Time.fixedDeltaTime;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Player>())
        {
            Destroy(this.gameObject);
        }
    }
}
