using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour
{
    public AudioClip Clip;
    public AudioMixerGroup Mixer;
    [Range(0, 1)]
    public float Volume = 1;
    public bool RandomizePitch;
    public Vector2 PitchLimits = new Vector2(0.8f, 1.1f);
    public AudioSource soundSource;
    public bool UseRandom;
    public AudioClip[] Clips;
    public bool AutoPlay;
    public float AutoPlayTime;
    float timer;

    private void Awake()
    {
        if (soundSource == null)
            soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.outputAudioMixerGroup = Mixer;
        soundSource.clip = Clip;
        soundSource.volume = Volume;

        if (AutoPlay)
            soundSource.Play();
    }

    public void EmitSound()
    {
        if (UseRandom)
        {
            int r = Random.Range(0, Clips.Length);
            soundSource.clip = Clips[r];
            if (RandomizePitch)
                UpdatePitch();
        }
        else
        {
            if (RandomizePitch)
                UpdatePitch();
        }
        soundSource.Play();
    }

    public void EmitSoundOneShot()
    {
        if (UseRandom)
        {
            int r = Random.Range(0, Clips.Length);
            soundSource.clip = Clips[r];
            if (RandomizePitch)
                UpdatePitch();
        }
        else
        {
            if (RandomizePitch)
                UpdatePitch();
        }
        soundSource.PlayOneShot(soundSource.clip);
    }

    void UpdatePitch()
    {
        soundSource.pitch = Random.Range(PitchLimits.x, PitchLimits.y);
    }

    void Update()
    {
        if (AutoPlay)
        {
            timer += Time.deltaTime;
            if (timer >= AutoPlayTime)
            {
                timer = 0;
                EmitSound();
            }
        }
    }

    public void SetClip(AudioClip clip)
    {
        soundSource.clip = clip;
    }
}
