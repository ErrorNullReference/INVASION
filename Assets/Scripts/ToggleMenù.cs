using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenù : MonoBehaviour
{

    private GameObject go;

    private void Awake()
    {
        go = transform.GetChild(0).gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (go.activeInHierarchy)
            {
                go.SetActive(false);
            }
            else
            {
                go.SetActive(true);

            }
        }
    }
}
