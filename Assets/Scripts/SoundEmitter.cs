using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour
{
    public AudioClip Clip;
    public AudioMixerGroup Mixer;
    public float SpatialValue;
    public float MaxDistanceVolume;
    public AnimationCurve VolumeCurve;
	[HideInInspector]
	public AudioSource soundSource;

    private void Awake()
    {
        soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.outputAudioMixerGroup = Mixer;
        soundSource.clip = Clip;
        soundSource.spatialBlend = SpatialValue;
		soundSource.maxDistance = MaxDistanceVolume;
        if (VolumeCurve.length != 0)
            soundSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, VolumeCurve);
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
