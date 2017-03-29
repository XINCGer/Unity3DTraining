using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attach this script to any object that has idle animations.
/// It's expected that the main idle loop animation is called "idle", and idle
/// break animations all begin with "idle" (ex: idleStretch, idleYawn, etc).
/// The script will place the idle loop animation on layer 0, and breaks on layer 1.
/// </summary>

[AddComponentMenu("NGUI/Examples/Play Idle Animations")]
public class PlayIdleAnimations : MonoBehaviour
{
	Animation mAnim;
	AnimationClip mIdle;
	List<AnimationClip> mBreaks = new List<AnimationClip>();
	float mNextBreak = 0f;
	int mLastIndex = 0;

	/// <summary>
	/// Find all idle animations.
	/// </summary>

	void Start ()
	{
		mAnim = GetComponentInChildren<Animation>();
		
		if (mAnim == null)
		{
			Debug.LogWarning(NGUITools.GetHierarchy(gameObject) + " has no Animation component");
			Destroy(this);
		}
		else
		{
			foreach (AnimationState state in mAnim)
			{
				if (state.clip.name == "idle")
				{
					state.layer = 0;
					mIdle = state.clip;
					mAnim.Play(mIdle.name);
				}
				else if (state.clip.name.StartsWith("idle"))
				{
					state.layer = 1;
					mBreaks.Add(state.clip);
				}
			}
		
			// No idle breaks found -- this script is unnecessary
			if (mBreaks.Count == 0) Destroy(this);
		}
	}

	/// <summary>
	/// If it's time to play a new idle break animation, do so.
	/// </summary>

	void Update ()
	{
		if (mNextBreak < Time.time)
		{
			if (mBreaks.Count == 1)
			{
				// Only one break animation
				AnimationClip clip = mBreaks[0];
				mNextBreak = Time.time + clip.length + Random.Range(5f, 15f);
				mAnim.CrossFade(clip.name);
			}
			else
			{
				int index = Random.Range(0, mBreaks.Count - 1);
				
				// Never choose the same animation twice in a row
				if (mLastIndex == index)
				{
					++index;
					if (index >= mBreaks.Count) index = 0;
				}

				mLastIndex = index;
				AnimationClip clip = mBreaks[index];
				mNextBreak = Time.time + clip.length + Random.Range(2f, 8f);
				mAnim.CrossFade(clip.name);
			}
		}
	}
}