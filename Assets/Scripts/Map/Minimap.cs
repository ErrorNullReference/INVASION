using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;

public class Minimap : MonoBehaviour
{
    public Camera camera;
    public RawImage Map;
    public SOListPlayerContainer Players;
    public Vector2 MapSize, WorldSize;
    Transform Player;
    Vector3 startPos;
    Vector2 mapStartPos;
    float multX, multY;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < Players.Elements.Count; i++)
        {
            if (Players[i].Avatar.UserInfo != null && Players[i].Avatar.UserInfo.SteamID == Client.MyID)
            {
                Player = Players[i].transform;
                break;
            }
        }

        camera.transform.position = Player.transform.position + new Vector3(0, 90, 0);
        camera.transform.eulerAngles = new Vector3(90, 180, 0);
        startPos = camera.transform.position;
        mapStartPos = Map.transform.position;

        multX = MapSize.x / WorldSize.x;
        multY = MapSize.y / WorldSize.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Player == null)
            return;

        camera.transform.position = Player.transform.position + new Vector3(0, 90, 0);
        Vector3 pos = startPos - camera.transform.position;
        Map.transform.position = mapStartPos - Convert(pos);
    }

    Vector2 Convert(Vector3 pos)
    {
        multX = MapSize.x / WorldSize.x;
        multY = MapSize.y / WorldSize.y;

        Vector2 p = new Vector2(pos.x * multX, pos.z * multY);
        return p;
    }
}
