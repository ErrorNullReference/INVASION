using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOPRO;
public class SwitchCamera : MonoBehaviour
{
    public SOListPlayerContainer Players;
    public KeyCode SwitchPlayer = KeyCode.Tab;
    public FollowPlayer Follow;

    private void Update()
    {
        if (Players.Elements.Count != 0 && Input.GetKeyDown(SwitchPlayer) || Players[Follow.CurrentIndex].Dead)
        {
            int index = Follow.CurrentIndex;
            int length = Players.Elements.Count;
            for (int i = 0; i < length; i++)
            {
                index++;
                if (index >= length)
                    index = 0;

                if (!Players[index].Dead)
                {
                    Follow.CurrentIndex = index;
                    break;
                }
            }
        }
    }
    public void IsInactive(bool isInactive)
    {
        this.enabled = !isInactive;
    }
    private void OnDisable()
    {
        Follow.SetFollowLocalPlayer();
    }
}