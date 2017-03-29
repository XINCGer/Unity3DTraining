//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the audio source's volume.
/// </summary>

[RequireComponent(typeof(AudioSource))]
[AddComponentMenu("NGUI/Tween/Tween Volume")]
public class TweenVolume : UITweener
{
	[Range(0f, 1f)] public float from = 1f;
	[Range(0f, 1f)] public float to = 1f;

	AudioSource mSource;

	/// <summary>
	/// Cached version of 'audio', as it's always faster to cache.
	/// </summary>

	public AudioSource audioSource
	{
		get
		{
			if (mSource == null)
			{
				mSource = GetComponent<AudioSource>();
				
				if (mSource == null)
				{
					mSource = GetComponent<AudioSource>();

					if (mSource == null)
					{
						Debug.LogError("TweenVolume needs an AudioSource to work with", this);
						enabled = false;
					}
				}
			}
			return mSource;
		}
	}

	[System.Obsolete("Use 'value' instead")]
	public float volume { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Audio source's current volume.
	/// </summary>

	public float value
	{
		get
		{
			return audioSource != null ? mSource.volume : 0f;
		}
		set
		{
			if (audioSource != null) mSource.volume = value;
		}
	}

	protected override void OnUpdate (float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
		mSource.enabled = (mSource.volume > 0.01f);
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenVolume Begin (GameObject go, float duration, float targetVolume)
	{
		TweenVolume comp = UITweener.Begin<TweenVolume>(go, duration);
		comp.from = comp.value;
		comp.to = targetVolume;

		if (targetVolume > 0f)
		{
			var s = comp.audioSource;
			s.enabled = true;
			s.Play();
		}
		return comp;
	}

	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
}
