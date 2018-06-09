using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AvatarsMgr : MonoBehaviour
{
    public SelectableAvatar[] Avatars;

    void Start()
    {
        SteamCallbackReceiver.LobbyDataUpdateEvent += UpdateUsersID;
        SteamCallbackReceiver.ChatUpdateEvent += UpdateUsersState;

        Client.AddCommand(PacketType.RequestAvatarSelection, ControlAvatarDisponibility);
        Client.AddCommand(PacketType.AnswerAvatarSelection, SetAvatar);
    }

    void OnEnable()
    {
        UpdateUsers();
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.LobbyDataUpdateEvent -= UpdateUsersID;
        SteamCallbackReceiver.ChatUpdateEvent -= UpdateUsersState;
    }

    void UpdateUsersState(LobbyChatUpdate_t cb)
    {
        UpdateUsers();
    }

    void UpdateUsersID(LobbyDataUpdate_t cb)
    {
        UpdateUsers();
    }

    void UpdateUsers()
    {
        for (int j = 0; j < Avatars.Length; j++)
            Avatars[j].Reset();

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
                Client.Users[i].AvatarID = index;

                for (int j = 0; j < Avatars.Length; j++)
                {
                    if (Avatars[j].avatarID == index)
                    {
                        Avatars[j].UpdateOwner(Client.Users[i]);
                        Avatars[j].Button.interactable = false;
                    }
                }
            }
        }
    }

    void ControlAvatarDisponibility(byte[] data, uint dataLenght, CSteamID sender)
    {
        int avatarIndex = (int)data[0];
        for (int i = 0; i < Client.Users.Count; i++)
        {
            string userIndex = SteamMatchmaking.GetLobbyMemberData(Client.Lobby.LobbyID, Client.Users[i].SteamID, "AvatarID");
            if (userIndex == "")
                continue;
            int index;
            if (int.TryParse(userIndex, out index))
            {
                if (index == avatarIndex)
                    return;
            }
        }

        byte[] d = ArrayPool<byte>.Get(1);
        d[0] = (byte)avatarIndex;

        Client.SendPacket(d, PacketType.AnswerAvatarSelection, Client.MyID, sender, EP2PSend.k_EP2PSendReliable);

        ArrayPool<byte>.Recycle(d);
    }

    void SetAvatar(byte[] data, uint dataLenght, CSteamID sender)
    {
        int avatarIndex = (int)data[0];

        SteamMatchmaking.SetLobbyMemberData(Client.Lobby.LobbyID, "AvatarID", avatarIndex.ToString());
    }
}
