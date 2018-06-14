using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class SelectableAvatar : MonoBehaviour
{
    public int avatarID;
    public Button Button, AddButton;
    public RawImage avatarImage;
    [HideInInspector]
    public User Owner;
    [SerializeField]
    Texture DefaultTexture;

    //void Update()
    //{
    //    timer += Time.deltaTime;
    //    if (timer >= 1)
    //    {
    //        timer = 0;
    //        if (!imageLoaded && OwnerID != new CSteamID(0))
    //        {
    //            Texture2D tex = GetUserAvatar(OwnerID);
    //            SetTex(tex);
    //        }
    //    }
    //}

    public void SelectAvatar()
    {
        if (Client.Users == null)
            return;

        for (int i = 0; i < Client.Users.Count; i++)
        {
            string data = SteamMatchmaking.GetLobbyMemberData(Client.Lobby.LobbyID, Client.Users[i].SteamID, "AvatarID");
            if (data == "")
                continue;
            int index;
            if (int.TryParse(data, out index))
            {
                if (index == avatarID)
                    return;
            }
        }
        byte[] d = ArrayPool<byte>.Get(1);
        d[0] = (byte)avatarID;

        Client.SendPacketToHost(d,0,1, PacketType.RequestAvatarSelection, EP2PSend.k_EP2PSendReliable);

        ArrayPool<byte>.Recycle(d);
    }

    public void UpdateOwner(User owner)
    {
        Owner = owner;
        SetTex(Owner.SteamAvatarImage);

        AddButton.gameObject.SetActive(false);
    }

    void SetTex(Texture2D tex)
    {
        if (tex != null)
            avatarImage.texture = tex;
        else avatarImage.texture = DefaultTexture; 
    }

    public void Reset()
    {
        Owner = null;
        avatarImage.texture = DefaultTexture;
        Button.interactable = true;
        AddButton.gameObject.SetActive(true);
    }
}
