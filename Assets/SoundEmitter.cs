using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour
{
    public AudioClip Clip;
    public AudioMixerGroup Mixer;
    private AudioSource soundSource;

    private void Awake()
    {
        soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.outputAudioMixerGroup = Mixer;
        soundSource.clip = Clip;
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
