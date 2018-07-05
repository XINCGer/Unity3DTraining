using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays the specified sound when clicked on.
/// </summary>

public class PlaySoundOnClick : MonoBehaviour
{
	public AudioClip audioClip;
	public float volume = 1f;
	public float pitch = 1f;

	void OnClickEvent ()
	{
		if (enabled)
		{
			PlaySound(audioClip, volume, pitch);
		}
	}
	
	static AudioListener mListener;
	static bool mLoaded = false;
	static float mGlobalVolume = 1f;

	static public float soundVolume
	{
		get
		{
			if (!mLoaded)
			{
				mLoaded = true;
				mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
			}
			return mGlobalVolume;
		}
		set
		{
			if (mGlobalVolume != value)
			{
				mLoaded = true;
				mGlobalVolume = value;
				PlayerPrefs.SetFloat("Sound", value);
			}
		}
	}
	
	static public AudioSource PlaySound (AudioClip clip) { return PlaySound(clip, 1f, 1f); }

	static public AudioSource PlaySound (AudioClip clip, float volume) { return PlaySound(clip, volume, 1f); }

	static public AudioSource PlaySound (AudioClip clip, float volume, float pitch)
	{
		volume *= soundVolume;

		if (clip != null && volume > 0.01f)
		{
			if (mListener == null)
			{
				mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;

				if (mListener == null)
				{
					Camera cam = Camera.main;
					if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
					if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
				}
			}

			if (mListener != null && mListener.enabled && mListener.gameObject.activeInHierarchy)
			{
				AudioSource source = mListener.GetComponent<AudioSource>();
				if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
				source.pitch = pitch;
				source.PlayOneShot(clip, volume);
				return source;
			}
		}
		return null;
	}
}
