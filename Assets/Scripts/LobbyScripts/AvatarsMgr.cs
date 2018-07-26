using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AvatarsMgr : MonoBehaviour
{
    [SerializeField]
    private SelectableAvatar SelectableAvatarTemplate;
    [SerializeField]
    private Texture[] AvatarsTextures;
    [SerializeField]
    private Animator[] AvatarsAnim;
    [SerializeField]
    [Range(0, 1)]
    private float posY;
    [SerializeField]
    private float distance;
    [HideInInspector]
    public SelectableAvatar[] Avatars;
    float time, timer;
    List<int> notActivatedAvatars;

    void Start()
    {
        SteamCallbackReceiver.LobbyDataUpdateEvent += UpdateUsersID;
        SteamCallbackReceiver.ChatUpdateEvent += UpdateUsersState;

        Client.AddCommand(PacketType.RequestAvatarSelection, ControlAvatarDisponibility);
        Client.AddCommand(PacketType.AnswerAvatarSelection, SetAvatar);

        Avatars = new SelectableAvatar[4];
        for (int i = 0; i < 4; i++)
        {
            Avatars[i] = Instantiate(SelectableAvatarTemplate, this.transform);
            Avatars[i].avatarID = i;
            Avatars[i].modelImage.texture = AvatarsTextures[i];
            //Avatars[i].transform.position = new Vector3(Screen.width / 8f * (2 * i + 1), Screen.height * posY, 0);
            Avatars[i].transform.position = new Vector3(distance * i + (Screen.width - distance * 3) / 2f, Screen.height * posY, 0);
        }

        timer = 1;
        notActivatedAvatars = new List<int>(Avatars.Length);
    }

    void OnEnable()
    {
        time = timer;
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

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            UpdateUsers();
            time = timer;
        }
        for (int i = 0; i < Avatars.Length; i++)
        {
            if (Avatars[i].Owner == null)
            {
                AvatarsAnim[i].SetBool("Selected", false);
            }
            else
            {
                AvatarsAnim[i].SetBool("Selected", true);

            }
        }
    }

    void UpdateUsers()
    {
        if (Client.Users == null || notActivatedAvatars == null)
            return;

        notActivatedAvatars.Clear();
        for (int i = 0; i < Avatars.Length; i++)
            notActivatedAvatars.Add(i);
        
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
                        notActivatedAvatars.Remove(j);
                    }
                }
            }
        }

        for (int j = 0; j < notActivatedAvatars.Count; j++)
        {
            if (Avatars[notActivatedAvatars[j]] == null)
                return;
            Avatars[notActivatedAvatars[j]].Reset();
        }
    }

    void ControlAvatarDisponibility(byte[] data, uint dataLenght, CSteamID sender)
    {
        int avatarIndex = data[0];
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

        Client.SendPacket(d, 0, d.Length, PacketType.AnswerAvatarSelection, Client.MyID, sender, EP2PSend.k_EP2PSendReliable);

        ArrayPool<byte>.Recycle(d);
    }

    void SetAvatar(byte[] data, uint dataLenght, CSteamID sender)
    {
        int avatarIndex = data[0];

        SteamMatchmaking.SetLobbyMemberData(Client.Lobby.LobbyID, "AvatarID", avatarIndex.ToString());
    }
}
