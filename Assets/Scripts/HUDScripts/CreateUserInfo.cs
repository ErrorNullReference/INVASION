using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUserInfo : MonoBehaviour
{
    public UserInfo Template;
	public bool MyPlayer;

    // Use this for initialization
    void Start()
    {
        SimpleAvatar avatar = GetComponent<SimpleAvatar>();
        if (avatar == null)
            return;

        int count = UserInfo.Count;

        UserInfo info = Instantiate(Template);

		float size = 0.7f;
		if (MyPlayer)
			size = 1;
		info.Create(avatar.UserInfo.SteamID, new Vector2(size, size), MyPlayer);
    }
}
