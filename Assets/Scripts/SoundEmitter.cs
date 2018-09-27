using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEmitter : MonoBehaviour
{
	public AudioClip Clip;
	public AudioMixerGroup Mixer;
	public bool RandomizePitch;
	public Vector2 PitchLimits = new Vector2(0.8f, 1.1f);
	[HideInInspector]
	public AudioSource soundSource;

	private void Awake ()
	{
		soundSource = gameObject.AddComponent<AudioSource> ();
		soundSource.outputAudioMixerGroup = Mixer;
		soundSource.clip = Clip;
	}

	public void EmitSound ()
	{
		if (RandomizePitch)
			UpdatePitch ();
		soundSource.Play ();
	}

	public void EmitSoundOneShot ()
	{
		soundSource.PlayOneShot (soundSource.clip);
	}

	void UpdatePitch ()
	{
		soundSource.pitch = Random.Range (PitchLimits.x, PitchLimits.y);
	}
}
