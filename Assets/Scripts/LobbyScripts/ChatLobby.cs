using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;
using UnityEngine.UI;

public class ChatLobby : MonoBehaviour
{
    public Text Text;
    public InputField InputField;

    // Use this for initialization
    void Start()
    {
        SteamCallbackReceiver.ChatMessageEvent += ReceiveChatMessage;
        //SteamCallbackReceiver.ChatUpdateEvent += UpdateUserStatus;
    }

    void OnEnable()
    {
        Text.text = "";
        InputField.text = "";
    }

    public void SendChatMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        SteamMatchmaking.SendLobbyChatMsg(Client.Lobby.LobbyID, data, data.Length);
        InputField.text = "";
    }

    void ReceiveChatMessage(LobbyChatMsg_t cb)
    {
        CSteamID user;
        byte[] data = new byte[4096];
        EChatEntryType chatType;
        int dataLenght = SteamMatchmaking.GetLobbyChatEntry((CSteamID)cb.m_ulSteamIDLobby, (int)cb.m_iChatID, out user, data, data.Length, out chatType);

        string name = SteamFriends.GetFriendPersonaName(user);

        if (chatType == EChatEntryType.k_EChatEntryTypeChatMsg)
        {
            string message = Encoding.UTF8.GetString(data, 0, dataLenght);
            Text.text += "\n" + name + " : " + message;
        }
    }

    void UpdateUserStatus(LobbyChatUpdate_t cb)
    {
        string name = SteamFriends.GetFriendPersonaName((CSteamID)cb.m_ulSteamIDUserChanged);
        EChatMemberStateChange chatType = (EChatMemberStateChange)cb.m_rgfChatMemberStateChange;
        
        switch (chatType)
        {
            case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
                Text.text += "\n" + name + " joined the lobby";
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
                Text.text += Text.text += "\n" + name + " left the lobby";
                break;
            case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
                Text.text += Text.text += "\n" + name + " left the lobby";
                break;
        }
    }

    void OnDestroy()
    {
        SteamCallbackReceiver.ChatMessageEvent -= ReceiveChatMessage;
        SteamCallbackReceiver.ChatUpdateEvent -= UpdateUserStatus;
    }
}
