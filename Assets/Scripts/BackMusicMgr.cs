using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackMusicMgr : MonoBehaviour
{
    [System.Serializable]
    public struct MusicSetup
    {
        public AudioClip Intro, Music;
    }

    public MusicSetup[] Musics;
    public AudioSource ASource;
    int r;

    // Use this for initialization
    void Start()
    {
        r = Random.Range(0, Musics.Length);
        if (Musics[r].Intro != null)
        {
            ASource.clip = Musics[r].Intro;
            ASource.Play();
            ASource.loop = false;
        }
        else
        {
            ASource.loop = true;
            ASource.clip = Musics[r].Music;
            ASource.Play();
        }
    }

    void Update()
    {
        if (!ASource.isPlaying)
        {
            ASource.loop = true;
            ASource.clip = Musics[r].Music;
            ASource.Play();
        }
    }
}
