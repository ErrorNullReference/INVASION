using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingPlayers : MonoBehaviour
{
    bool start;
    public int fontSize;

    // Use this for initialization
    void Start()
    {
        Client.OnGameEnd += () => start = false;
    }

    void Destroy()
    {
        Client.OnGameEnd -= () => start = false;
    }

    public void StartGame()
    {
        start = true;
    }

    void OnGUI()
    {
        if (start)
            return;

        Color c = GUI.color;
        GUI.skin.label.fontSize = fontSize;
        GUILayout.Label("Waiting for players");

        if (Client.Users == null)
            return;
        for (int i = 0; i < Client.Users.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Client.Users[i].SteamUsername + " : ");
            if (Client.Users[i].InGame)
            {
                GUI.color = Color.green;
                GUILayout.Label("Ready");

            }
            else
            {
                GUI.color = Color.red;
                GUILayout.Label("Waiting");

            }
            GUI.color = c;
            GUILayout.EndHorizontal();
        }
    }
}
