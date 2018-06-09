using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;
using UnityEngine.UI;
using GENUtility;
public class ChatLobby : MonoBehaviour
{
    public Text Text;
    public InputField InputField;
    private readonly byte[] chatData = new byte[4096];//TODO: maybe even static?
    private readonly byte[] inputData = new byte[1024];//TODO: maybe even static?
    private readonly Encoding encoder = Encoding.UTF8;

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
        int n = ByteManipulator.Write(inputData, 0, message, encoder);
        SteamMatchmaking.SendLobbyChatMsg(Client.Lobby.LobbyID, inputData, n);
        InputField.text = "";
    }

    void ReceiveChatMessage(LobbyChatMsg_t cb)
    {
        CSteamID user;
        EChatEntryType chatType;
        int dataLenght = SteamMatchmaking.GetLobbyChatEntry((CSteamID)cb.m_ulSteamIDLobby, (int)cb.m_iChatID, out user, chatData, chatData.Length, out chatType);

        string name = SteamFriends.GetFriendPersonaName(user);

        if (chatType == EChatEntryType.k_EChatEntryTypeChatMsg)
        {
            int n;
            string message = ByteManipulator.ReadString(chatData, 0, encoder, out n);
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
