using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUserInfo : MonoBehaviour
{
    public UserInfo Template;

    // Use this for initialization
    void Start()
    {
        SimpleAvatar avatar = GetComponent<SimpleAvatar>();
        if (avatar == null)
            return;

        int count = UserInfo.Count;

        UserInfo info = Instantiate(Template);
        info.Create(avatar.UserInfo.SteamID, new Vector2(0.7f, 0.7f));
    }
}
