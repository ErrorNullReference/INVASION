using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    public float UpdateTime;
    Text text;
    int index;
    float time;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }
	
    // Update is called once per frame
    void Update()
    {
        if (text == null)
            return;

        time += Time.deltaTime;
        if (time < UpdateTime)
            return;
        time = 0;        

        switch (index)
        {
            case 0:
                text.text = "Loading";
                break;
            case 1:
                text.text = "Loading.";
                break;
            case 2:
                text.text = "Loading..";
                break;
            case 3:
                text.text = "Loading...";
                break;
        }

        index = (index + 1) % 4;
    }
}
