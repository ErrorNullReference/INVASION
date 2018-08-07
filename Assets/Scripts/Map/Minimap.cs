using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SOPRO;

public class Minimap : MonoBehaviour
{
    public Camera camera;
    public RawImage PlayerView;
    public Shader unlitShader;
    public SOListPlayerContainer Players;
    public float LerpSpeed;
    public bool RotateCamera, RotateView;
    Transform Player;
    Vector3 offset;
    Quaternion startRot;

    // Use this for initialization
    void Start()
    {
        //camera.SetReplacementShader(unlitShader, "");

        offset = camera.transform.position;

        for (int i = 0; i < Players.Elements.Count; i++)
        {
            if (Players[i].Avatar.UserInfo.SteamID == Client.MyID)
            {
                Player = Players[i].transform;
                break;
            }
        }

        Quaternion rotation = Quaternion.Euler(new Vector3(0, Camera.main.transform.eulerAngles.y, 0));
        Vector3 forward = rotation * new Vector3(0, 0, 1);

        startRot = Quaternion.LookRotation(forward);
        camera.transform.rotation = startRot * Quaternion.Euler(90, 0, 0);
        PlayerView.transform.rotation = startRot;
    }

    void OnValidate()
    {
        if (!RotateCamera)
            camera.transform.rotation = startRot * Quaternion.Euler(90, 0, 0);
        else
            PlayerView.transform.rotation = Quaternion.Euler(0, startRot.eulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
            return;

        camera.transform.position = Player.position + offset;
        if (RotateCamera)
            camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, Quaternion.Euler(90, Player.eulerAngles.y, 0), Time.deltaTime * LerpSpeed);
        else if (RotateView)
            PlayerView.transform.rotation = Quaternion.Slerp(PlayerView.transform.rotation, Quaternion.Euler(0, 0, -Player.eulerAngles.y + startRot.eulerAngles.y), Time.deltaTime * LerpSpeed);
    }
}
