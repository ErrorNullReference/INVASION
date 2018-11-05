using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UpdateLobbyAudio : MonoBehaviour
{
    public AudioClip StartClip, SelectionClip;

    AudioSource ASource;

    // Use this for initialization
    void Start()
    {
        ASource = GetComponent<AudioSource>();

        MenuMgr.OnStart += () => SetClip(StartClip);
        MenuMgr.OnAvatarSelection += () => SetClip(SelectionClip);
    }

    void OnDestroy()
    {
        MenuMgr.OnStart -= () => SetClip(StartClip);
        MenuMgr.OnAvatarSelection -= () => SetClip(SelectionClip);
    }

    void SetClip(AudioClip clip)
    {
        ASource.clip = clip;
        ASource.Play();
    }
}
