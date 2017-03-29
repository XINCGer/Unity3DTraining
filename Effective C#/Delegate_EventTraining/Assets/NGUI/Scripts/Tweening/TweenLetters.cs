//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attaching this script to a label will make the label's letters animate.
/// </summary>

public class TweenLetters : UITweener
{
	public enum AnimationLetterOrder { Forward, Reverse, Random }

	class LetterProperties
	{
		public float start;
		public float duration; // if RandomDurations is set, these will all be different.
		public Vector2 offset;
	}

	[System.Serializable]
	public class AnimationProperties
	{
		public AnimationLetterOrder animationOrder = AnimationLetterOrder.Random;
		[Range(0f, 1f)]
		public float overlap = 0.5f;

		public bool randomDurations = false;
		[MinMaxRange(0f, 1f)]
		public Vector2 randomness = new Vector2(0.25f, 0.75f);

		public Vector2 offsetRange = Vector2.zero;
		public Vector3 pos = Vector3.zero;
		public Vector3 rot = Vector3.zero;
		public Vector3 scale = Vector3.one;
		public float alpha = 1f;
	}

	public AnimationProperties hoverOver;
	public AnimationProperties hoverOut;

	UILabel mLabel;
	int mLastLen = -1;
	int[] mLetterOrder;
	LetterProperties[] mLetter;
	AnimationProperties mCurrent;

	void OnEnable ()
	{
		mLastLen = -1;
		mLabel.onPostFill += OnPostFill;
	}

	void OnDisable ()
	{
		mLabel.onPostFill -= OnPostFill;
	}

	void Awake ()
	{
		mLabel = GetComponent<UILabel>();
		mCurrent = hoverOver;
	}

	public override void Play (bool forward)
	{
		mCurrent = (forward) ? hoverOver : hoverOut;
		base.Play(forward);
	}

	void OnPostFill (UIWidget widget, int bufferOffset, List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		if (verts == null || verts.Count == 0) return;
		if (mLabel == null) return;

		var quads = mLabel.quadsPerCharacter;
		const int quadVerts = 4;
		var characterCount = verts.Count / quads / quadVerts;

		var pt = mLabel.printedText;
		var newLen = string.IsNullOrEmpty(pt) ? 0 : pt.Length;

		if (mLastLen != newLen)
		{
			mLastLen = newLen;
			SetLetterOrder(characterCount);
			GetLetterDuration(characterCount);
		}

		var mtx = Matrix4x4.identity;
		var lerpPos = Vector3.zero;
		var lerpRot = Quaternion.identity;
		var lerpScale = Vector3.one;
		var lerpAlpha = 1f;
		int firstVert, letter;
		float letterStart, t; // The individual letters tweenFactor
		var letterCenter = Vector3.zero;
		var qRot = Quaternion.Euler(mCurrent.rot);
		var vert = Vector3.zero;
		var c = Color.clear;
		var timeIntoAnimation = base.tweenFactor * base.duration;

		for (int q = 0; q < quads; ++q)
		{
			for (int i = 0; i < characterCount; ++i)
			{
				letter = mLetterOrder[i]; // Choose which letter to animate.
				firstVert = q * characterCount * quadVerts + letter * quadVerts;

				if (firstVert > verts.Count)
				{
					Debug.LogError("TweenLetters encountered an unhandled case trying to modify a vertex " + firstVert + ". Vertex Count: " + verts.Count + " Pass: " + q + "\nText: " + pt);
					continue;
				}
				
				letterStart = mLetter[letter].start;
				t = Mathf.Clamp(timeIntoAnimation - letterStart, 0f, mLetter[letter].duration) / mLetter[letter].duration;
				t = animationCurve.Evaluate(t);

				letterCenter = GetCenter(verts, firstVert, quadVerts);

				var v = mLetter[letter].offset;
#if UNITY_4_7
				lerpPos = LerpUnclamped(mCurrent.pos + new Vector3(v.x, v.y, 0f), Vector3.zero, t);
				lerpRot = Quaternion.Slerp(qRot, Quaternion.identity, t);
				lerpScale = LerpUnclamped(mCurrent.scale, Vector3.one, t);
				lerpAlpha = LerpUnclamped(mCurrent.alpha, 1f, t);
#else
				lerpPos = Vector3.LerpUnclamped(mCurrent.pos + new Vector3(v.x, v.y, 0f), Vector3.zero, t);
				lerpRot = Quaternion.SlerpUnclamped(qRot, Quaternion.identity, t);
				lerpScale = Vector3.LerpUnclamped(mCurrent.scale, Vector3.one, t);
				lerpAlpha = Mathf.LerpUnclamped(mCurrent.alpha, 1f, t);
#endif
				mtx.SetTRS(lerpPos, lerpRot, lerpScale);

				for (int iv = firstVert; iv < firstVert + quadVerts; ++iv)
				{
					vert = verts[iv];
					vert -= letterCenter;
					vert = mtx.MultiplyPoint3x4(vert);
					vert += letterCenter;
					verts[iv] = vert;

					c = cols[iv];
					c.a = lerpAlpha;
					cols[iv] = c;
				}
			}
		}
	}

#if UNITY_4_7
	static Vector3 LerpUnclamped (Vector3 a, Vector3 b, float f)
	{
		a.x = a.x + (b.x - a.x) * f;
		a.y = a.y + (b.y - a.y) * f;
		a.z = a.z + (b.z - a.z) * f;
		return a;
	}

	static float LerpUnclamped (float a, float b, float f) { return a + (b - a) * f; }
#endif

	/// <summary>
	/// Check every frame to see if the text has changed and mark the label as having been updated.
	/// </summary>
	
	protected override void OnUpdate (float factor, bool isFinished)
	{
		mLabel.MarkAsChanged();
	}

	/// <summary>
	/// Sets the sequence that the letters are animated in.
	/// </summary>
	
	void SetLetterOrder (int letterCount)
	{
		if (letterCount == 0)
		{
			mLetter = null;
			mLetterOrder = null;
			return;
		}

		mLetterOrder = new int[letterCount];
		mLetter = new LetterProperties[letterCount];

		for (int i = 0; i < letterCount; ++i)
		{
			mLetterOrder[i] = (mCurrent.animationOrder == AnimationLetterOrder.Reverse) ? letterCount - 1 - i : i;

			int current = mLetterOrder[i];
			mLetter[current] = new LetterProperties();
			mLetter[current].offset = new Vector2(Random.Range(-mCurrent.offsetRange.x, mCurrent.offsetRange.x), Random.Range(-mCurrent.offsetRange.y, mCurrent.offsetRange.y));
		}

		if (mCurrent.animationOrder == AnimationLetterOrder.Random)
		{
			// Shuffle the numbers in the array.
			var rng = new System.Random();
			int n = letterCount;

			while (n > 1)
			{
				int k = rng.Next(--n + 1);
				int tmp = mLetterOrder[k];
				mLetterOrder[k] = mLetterOrder[n];
				mLetterOrder[n] = tmp;
			}
		}
	}

	/// <summary>
	/// Returns how long each letter has to animate based on the overall duration requested and how much they overlap.
	/// </summary>
	
	void GetLetterDuration (int letterCount)
	{
		if (mCurrent.randomDurations)
		{
			for (int i = 0; i < mLetter.Length; ++i)
			{
				mLetter[i].start = Random.Range(0f, mCurrent.randomness.x * base.duration);
				float end = Random.Range(mCurrent.randomness.y * base.duration, base.duration);
				mLetter[i].duration = end - mLetter[i].start;
			}
		}
		else
		{
			// Calculate how long each letter will take to fade in.
			float lengthPerLetter = base.duration / (float)letterCount;
			float flippedOverlap = 1f - mCurrent.overlap;

			// Figure out how long the animation will be taking into account overlapping letters.
			float totalDuration = lengthPerLetter * letterCount * flippedOverlap;

			// Scale the smaller total running time back up to the requested animation time.
			float letterDuration = ScaleRange(lengthPerLetter, totalDuration + lengthPerLetter * mCurrent.overlap, base.duration);

			float offset = 0;
			for (int i = 0; i < mLetter.Length; ++i)
			{
				int letter = mLetterOrder[i];
				mLetter[letter].start = offset;
				mLetter[letter].duration = letterDuration;
				offset += mLetter[letter].duration * flippedOverlap;
			}
		}
	}

	/// <summary>
	/// Simplified Scale range function that assumes a minimum of 0 for both ranges.
	/// </summary>

	float ScaleRange (float value, float baseMax, float limitMax)
	{
		return (limitMax * value / baseMax);
	}

	/// <summary>
	/// Finds the center point of a series of verts.
	/// </summary>

	static Vector3 GetCenter (List<Vector3> verts, int firstVert, int length)
	{
		Vector3 center = verts[firstVert];
		for (int v = firstVert + 1; v < firstVert + length; ++v) center += verts[v];
		return center / length;
	}
}
