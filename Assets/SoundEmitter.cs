using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour {

    private AudioSource soundSource;

    private void Awake()
    {
        soundSource = GetComponent<AudioSource>();
    }

    public void EmitSound()
    {
        soundSource.Play();
    }
    public void EmitSoundOneShot()
    {
        soundSource.PlayOneShot(soundSource.clip);
    }
}
