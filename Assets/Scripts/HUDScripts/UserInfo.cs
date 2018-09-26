using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class UserInfo : MonoBehaviour
{
    [HideInInspector]
    public CSteamID ID;
    [SerializeField]
    private Transform staticRoot, dinamicRoot;
    [SerializeField]
    [Range(0, 1)]
    private float Height;
    [SerializeField]
    [Range(0, 1)]
    private float YOffset;
    [SerializeField]
    [Range(0, 1)]
    private float XOffset;
    private Vector2 rootOffset;
    private Vector3 size;

    public static int Count = 0;

    void Start()
    {
        Client.OnUserDisconnected += DestroyOnDisconnection;
    }

    public void Create(CSteamID id, Vector2 size, bool myPlayer)
    {
        this.ID = id;
        rootOffset = staticRoot.position - dinamicRoot.position;
        Height = Height * (float)Screen.height;
        YOffset = YOffset * (float)Screen.height;
        XOffset = XOffset * (float)Screen.height;
        if (!myPlayer)
            Count++;
        SetScale(size);
        CalculatePosition(myPlayer);
    }

    void CalculatePosition(bool myPlayer)
    {
        if (myPlayer)
            SetPosition(new Vector2(XOffset, Screen.height - YOffset));
        else
            SetPosition(new Vector2(XOffset, Screen.height - YOffset - UserInfo.Count * (Height * size.y)));
    }

    public void SetPosition(Vector2 position)
    {
        staticRoot.position = position;
        dinamicRoot.position = position - rootOffset;
    }

    public void SetScale(Vector2 size)
    {
        this.size = new Vector3(size.x, size.y, 1);
        staticRoot.transform.localScale = this.size;
        dinamicRoot.transform.localScale = this.size;
    }

    void DestroyOnDisconnection(CSteamID id)
    {
        if (this.ID == id)
        {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        Client.OnUserDisconnected -= DestroyOnDisconnection;
        Count--;
    }
}
