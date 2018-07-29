using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Text;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public Text Text;
    public InputField InputField;
    private byte[] inputData;
    StringBuilder builder;
    bool justClosed;

    // Use this for initialization
    void Start()
    {
        Client.AddCommand(PacketType.Chat, ReceiveChatMessage);
        builder = new StringBuilder();
    }

    void OnEnable()
    {
        Text.text = "";
        InputField.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!justClosed)
            {
                InputField.gameObject.SetActive(true);
                InputField.Select();
                InputField.ActivateInputField();
            }
        }
        else if (justClosed)
            justClosed = false;
    }

    public void SendChatMessage(string message)
    {
        if (message.Length != 0)
        {
            inputData = Encoding.UTF8.GetBytes(message);
            Client.SendPacketToInGameUsers(inputData, 0, inputData.Length, PacketType.Chat, EP2PSend.k_EP2PSendReliable, true);
        }
        InputField.text = "";
        InputField.gameObject.SetActive(false);
        justClosed = true;
    }

    void ReceiveChatMessage(byte[] data, uint length, CSteamID sender)
    {
        string name = SteamFriends.GetFriendPersonaName(sender);
        string message = Encoding.UTF8.GetString(data, 0, (int)length);

        builder.Remove(0, builder.ToString().Length);
        builder.Append(Text.text);
        builder.Append(name);
        builder.Append(" : ");
        builder.Append(message);
        builder.Append("\n");

        Text.text = RemoveExcessLines(builder.ToString(), 3);
    }

    string RemoveExcessLines(string text, int maxLines)
    {
        int lines = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
                lines++;
        }

        if (lines <= maxLines)
            return text;

        int linesToRemove = lines - maxLines;
        lines = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n')
            {
                lines++;
                if (lines == linesToRemove)
                {
                    return text.Remove(0, i);
                }
            }
        }

        return text;
    }
}
