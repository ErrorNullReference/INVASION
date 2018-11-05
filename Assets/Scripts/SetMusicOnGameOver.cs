using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SetMusicOnGameOver : MonoBehaviour
{
    public AudioClip GameOverClip;

    AudioSource ASource;

    // Use this for initialization
    void Start()
    {
        ASource = GetComponent<AudioSource>();
        Client.OnGameEnd += SetClip;
    }

    void OnDestroy()
    {
        Client.OnGameEnd -= SetClip;
    }

    void SetClip()
    {
        ASource.clip = GameOverClip;
        ASource.Play();
    }
}
