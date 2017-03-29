//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Helper class containing generic functions used throughout the UI library.
/// </summary>

static public class NGUIMath
{
	/// <summary>
	/// Lerp function that doesn't clamp the 'factor' in 0-1 range.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public float Lerp (float from, float to, float factor) { return from * (1f - factor) + to * factor; }

	/// <summary>
	/// Clamp the specified integer to be between 0 and below 'max'.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public int ClampIndex (int val, int max) { return (val < 0) ? 0 : (val < max ? val : max - 1); }

	/// <summary>
	/// Wrap the index using repeating logic, so that for example +1 past the end means index of '1'.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public int RepeatIndex (int val, int max)
	{
		if (max < 1) return 0;
		while (val < 0) val += max;
		while (val >= max) val -= max;
		return val;
	}

	/// <summary>
	/// Ensure that the angle is within -180 to 180 range.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public float WrapAngle (float angle)
	{
		while (angle > 180f) angle -= 360f;
		while (angle < -180f) angle += 360f;
		return angle;
	}

	/// <summary>
	/// In the shader, equivalent function would be 'fract'
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public float Wrap01 (float val) { return val - Mathf.FloorToInt(val); }

	/// <summary>
	/// Convert a hexadecimal character to its decimal value.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public int HexToDecimal (char ch)
	{
		switch (ch)
		{
			case '0': return 0x0;
			case '1': return 0x1;
			case '2': return 0x2;
			case '3': return 0x3;
			case '4': return 0x4;
			case '5': return 0x5;
			case '6': return 0x6;
			case '7': return 0x7;
			case '8': return 0x8;
			case '9': return 0x9;
			case 'a':
			case 'A': return 0xA;
			case 'b':
			case 'B': return 0xB;
			case 'c':
			case 'C': return 0xC;
			case 'd':
			case 'D': return 0xD;
			case 'e':
			case 'E': return 0xE;
			case 'f':
			case 'F': return 0xF;
		}
		return 0xF;
	}

	/// <summary>
	/// Convert a single 0-15 value into its hex representation.
	/// It's coded because int.ToString(format) syntax doesn't seem to be supported by Unity's Flash. It just silently crashes.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public char DecimalToHexChar (int num)
	{
		if (num > 15) return 'F';
		if (num < 10) return (char)('0' + num);
		return (char)('A' + num - 10);
	}

	/// <summary>
	/// Convert a decimal value to its hex representation.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string DecimalToHex8 (int num)
	{
		num &= 0xFF;
#if UNITY_FLASH
		StringBuilder sb = new StringBuilder();
		sb.Append(DecimalToHexChar((num >> 4) & 0xF));
		sb.Append(DecimalToHexChar(num & 0xF));
		return sb.ToString();
#else
		return num.ToString("X2");
#endif
	}

	/// <summary>
	/// Convert a decimal value to its hex representation.
	/// It's coded because num.ToString("X6") syntax doesn't seem to be supported by Unity's Flash. It just silently crashes.
	/// string.Format("{0,6:X}", num).Replace(' ', '0') doesn't work either. It returns the format string, not the formatted value.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string DecimalToHex24 (int num)
	{
		num &= 0xFFFFFF;
#if UNITY_FLASH
		StringBuilder sb = new StringBuilder();
		sb.Append(DecimalToHexChar((num >> 20) & 0xF));
		sb.Append(DecimalToHexChar((num >> 16) & 0xF));
		sb.Append(DecimalToHexChar((num >> 12) & 0xF));
		sb.Append(DecimalToHexChar((num >> 8) & 0xF));
		sb.Append(DecimalToHexChar((num >> 4) & 0xF));
		sb.Append(DecimalToHexChar(num & 0xF));
		return sb.ToString();
#else
		return num.ToString("X6");
#endif
	}

	/// <summary>
	/// Convert a decimal value to its hex representation.
	/// It's coded because num.ToString("X6") syntax doesn't seem to be supported by Unity's Flash. It just silently crashes.
	/// string.Format("{0,6:X}", num).Replace(' ', '0') doesn't work either. It returns the format string, not the formatted value.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string DecimalToHex32 (int num)
	{
#if UNITY_FLASH
		StringBuilder sb = new StringBuilder();
		sb.Append(DecimalToHexChar((num >> 28) & 0xF));
		sb.Append(DecimalToHexChar((num >> 24) & 0xF));
		sb.Append(DecimalToHexChar((num >> 20) & 0xF));
		sb.Append(DecimalToHexChar((num >> 16) & 0xF));
		sb.Append(DecimalToHexChar((num >> 12) & 0xF));
		sb.Append(DecimalToHexChar((num >> 8) & 0xF));
		sb.Append(DecimalToHexChar((num >> 4) & 0xF));
		sb.Append(DecimalToHexChar(num & 0xF));
		return sb.ToString();
#else
		return num.ToString("X8");
#endif
	}

	/// <summary>
	/// Convert the specified color to RGBA32 integer format.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public int ColorToInt (Color c)
	{
		int retVal = 0;
		retVal |= Mathf.RoundToInt(c.r * 255f) << 24;
		retVal |= Mathf.RoundToInt(c.g * 255f) << 16;
		retVal |= Mathf.RoundToInt(c.b * 255f) << 8;
		retVal |= Mathf.RoundToInt(c.a * 255f);
		return retVal;
	}

	/// <summary>
	/// Convert the specified RGBA32 integer to Color.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public Color IntToColor (int val)
	{
		float inv = 1f / 255f;
		Color c = Color.black;
		c.r = inv * ((val >> 24) & 0xFF);
		c.g = inv * ((val >> 16) & 0xFF);
		c.b = inv * ((val >> 8) & 0xFF);
		c.a = inv * (val & 0xFF);
		return c;
	}

	/// <summary>
	/// Convert the specified integer to a human-readable string representing the binary value. Useful for debugging bytes.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public string IntToBinary (int val, int bits)
	{
		string final = "";

		for (int i = bits; i > 0; )
		{
			if (i == 8 || i == 16 || i == 24) final += " ";
			final += ((val & (1 << --i)) != 0) ? '1' : '0';
		}
		return final;
	}

	/// <summary>
	/// Convenience conversion function, allowing hex format (0xRrGgBbAa).
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static public Color HexToColor (uint val)
	{
		return IntToColor((int)val);
	}

	/// <summary>
	/// Convert from top-left based pixel coordinates to bottom-left based UV coordinates.
	/// </summary>

	static public Rect ConvertToTexCoords (Rect rect, int width, int height)
	{
		Rect final = rect;

		if (width != 0f && height != 0f)
		{
			final.xMin = rect.xMin / width;
			final.xMax = rect.xMax / width;
			final.yMin = 1f - rect.yMax / height;
			final.yMax = 1f - rect.yMin / height;
		}
		return final;
	}

	/// <summary>
	/// Convert from bottom-left based UV coordinates to top-left based pixel coordinates.
	/// </summary>

	static public Rect ConvertToPixels (Rect rect, int width, int height, bool round)
	{
		Rect final = rect;

		if (round)
		{
			final.xMin = Mathf.RoundToInt(rect.xMin * width);
			final.xMax = Mathf.RoundToInt(rect.xMax * width);
			final.yMin = Mathf.RoundToInt((1f - rect.yMax) * height);
			final.yMax = Mathf.RoundToInt((1f - rect.yMin) * height);
		}
		else
		{
			final.xMin = rect.xMin * width;
			final.xMax = rect.xMax * width;
			final.yMin = (1f - rect.yMax) * height;
			final.yMax = (1f - rect.yMin) * height;
		}
		return final;
	}

	/// <summary>
	/// Round the pixel rectangle's dimensions.
	/// </summary>

	static public Rect MakePixelPerfect (Rect rect)
	{
		rect.xMin = Mathf.RoundToInt(rect.xMin);
		rect.yMin = Mathf.RoundToInt(rect.yMin);
		rect.xMax = Mathf.RoundToInt(rect.xMax);
		rect.yMax = Mathf.RoundToInt(rect.yMax);
		return rect;
	}

	/// <summary>
	/// Round the texture coordinate rectangle's dimensions.
	/// </summary>

	static public Rect MakePixelPerfect (Rect rect, int width, int height)
	{
		rect = ConvertToPixels(rect, width, height, true);
		rect.xMin = Mathf.RoundToInt(rect.xMin);
		rect.yMin = Mathf.RoundToInt(rect.yMin);
		rect.xMax = Mathf.RoundToInt(rect.xMax);
		rect.yMax = Mathf.RoundToInt(rect.yMax);
		return ConvertToTexCoords(rect, width, height);
	}

	/// <summary>
	/// Constrain 'rect' to be within 'area' as much as possible, returning the Vector2 offset necessary for this to happen.
	/// This function is useful when trying to restrict one area (window) to always be within another (viewport).
	/// </summary>

	static public Vector2 ConstrainRect (Vector2 minRect, Vector2 maxRect, Vector2 minArea, Vector2 maxArea)
	{
		Vector2 offset = Vector2.zero;

		float contentX = maxRect.x - minRect.x;
		float contentY = maxRect.y - minRect.y;

		float areaX = maxArea.x - minArea.x;
		float areaY = maxArea.y - minArea.y;

		if (contentX > areaX)
		{
			float diff = contentX - areaX;
			minArea.x -= diff;
			maxArea.x += diff;
		}

		if (contentY > areaY)
		{
			float diff = contentY - areaY;
			minArea.y -= diff;
			maxArea.y += diff;
		}

		if (minRect.x < minArea.x) offset.x += minArea.x - minRect.x;
		if (maxRect.x > maxArea.x) offset.x -= maxRect.x - maxArea.x;
		if (minRect.y < minArea.y) offset.y += minArea.y - minRect.y;
		if (maxRect.y > maxArea.y) offset.y -= maxRect.y - maxArea.y;
		
		return offset;
	}

	/// <summary>
	/// Calculate the combined bounds of all widgets attached to the specified game object or its children (in world space).
	/// </summary>

	static public Bounds CalculateAbsoluteWidgetBounds (Transform trans)
	{
		if (trans != null)
		{
			UIWidget[] widgets = trans.GetComponentsInChildren<UIWidget>() as UIWidget[];
			if (widgets.Length == 0) return new Bounds(trans.position, Vector3.zero);

			Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			Vector3 v;

			for (int i = 0, imax = widgets.Length; i < imax; ++i)
			{
				UIWidget w = widgets[i];
				if (!w.enabled) continue;

				Vector3[] corners = w.worldCorners;

				for (int j = 0; j < 4; ++j)
				{
					v = corners[j];

					if (v.x > vMax.x) vMax.x = v.x;
 					if (v.y > vMax.y) vMax.y = v.y;
 					if (v.z > vMax.z) vMax.z = v.z;
 
 					if (v.x < vMin.x) vMin.x = v.x;
 					if (v.y < vMin.y) vMin.y = v.y;
 					if (v.z < vMin.z) vMin.z = v.z;
				}
			}

			Bounds b = new Bounds(vMin, Vector3.zero);
			b.Encapsulate(vMax);
			return b;
		}
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	/// <summary>
	/// Calculate the combined bounds of all widgets attached to the specified game object or its children (in relative-to-object space).
	/// </summary>

	static public Bounds CalculateRelativeWidgetBounds (Transform trans)
	{
		return CalculateRelativeWidgetBounds(trans, trans, false);
	}

	/// <summary>
	/// Calculate the combined bounds of all widgets attached to the specified game object or its children (in relative-to-object space).
	/// </summary>

	static public Bounds CalculateRelativeWidgetBounds (Transform trans, bool considerInactive)
	{
		return CalculateRelativeWidgetBounds(trans, trans, considerInactive);
	}

	/// <summary>
	/// Calculate the combined bounds of all widgets attached to the specified game object or its children (in relative-to-object space).
	/// </summary>

	static public Bounds CalculateRelativeWidgetBounds (Transform relativeTo, Transform content)
	{
		return CalculateRelativeWidgetBounds(relativeTo, content, false);
	}

	/// <summary>
	/// Calculate the combined bounds of all widgets attached to the specified game object or its children (in relative-to-object space).
	/// </summary>

	static public Bounds CalculateRelativeWidgetBounds (Transform relativeTo, Transform content, bool considerInactive, bool considerChildren = true)
	{
		if (content != null && relativeTo != null)
		{
			bool isSet = false;
			Matrix4x4 toLocal = relativeTo.worldToLocalMatrix;
			Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
			CalculateRelativeWidgetBounds(content, considerInactive, true, ref toLocal, ref min, ref max, ref isSet, considerChildren);

			if (isSet)
			{
				Bounds b = new Bounds(min, Vector3.zero);
				b.Encapsulate(max);
				return b;
			}
		}
		return new Bounds(Vector3.zero, Vector3.zero);
	}

	/// <summary>
	/// Recursive function used to calculate the widget bounds.
	/// </summary>

	[System.Diagnostics.DebuggerHidden]
	[System.Diagnostics.DebuggerStepThrough]
	static void CalculateRelativeWidgetBounds (Transform content, bool considerInactive, bool isRoot,
		ref Matrix4x4 toLocal, ref Vector3 vMin, ref Vector3 vMax, ref bool isSet, bool considerChildren)
	{
		if (content == null) return;
		if (!considerInactive && !NGUITools.GetActive(content.gameObject)) return;

		// If this isn't a root node, check to see if there is a panel present
		UIPanel p = isRoot ? null : content.GetComponent<UIPanel>();

		// Ignore disabled panels as a disabled panel means invisible children
		if (p != null && !p.enabled) return;

		// If there is a clipped panel present simply include its dimensions
		if (p != null && p.clipping != UIDrawCall.Clipping.None)
		{
			Vector3[] corners = p.worldCorners;

			for (int j = 0; j < 4; ++j)
			{
				Vector3 v = toLocal.MultiplyPoint3x4(corners[j]);

				if (v.x > vMax.x) vMax.x = v.x;
 				if (v.y > vMax.y) vMax.y = v.y;
 				if (v.z > vMax.z) vMax.z = v.z;
 
 				if (v.x < vMin.x) vMin.x = v.x;
 				if (v.y < vMin.y) vMin.y = v.y;
 				if (v.z < vMin.z) vMin.z = v.z;

				isSet = true;
			}
		}
		else // No panel present
		{
			// If there is a widget present, include its bounds
			UIWidget w = content.GetComponent<UIWidget>();

			if (w != null && w.enabled)
			{
				Vector3[] corners = w.worldCorners;

				for (int j = 0; j < 4; ++j)
				{
					Vector3 v = toLocal.MultiplyPoint3x4(corners[j]);

					if (v.x > vMax.x) vMax.x = v.x;
					if (v.y > vMax.y) vMax.y = v.y;
					if (v.z > vMax.z) vMax.z = v.z;

					if (v.x < vMin.x) vMin.x = v.x;
					if (v.y < vMin.y) vMin.y = v.y;
					if (v.z < vMin.z) vMin.z = v.z;

					isSet = true;
				}

				if (!considerChildren) return;
			}
			
			for (int i = 0, imax = content.childCount; i < imax; ++i)
				CalculateRelativeWidgetBounds(content.GetChild(i), considerInactive, false, ref toLocal, ref vMin, ref vMax, ref isSet, true);
		}
	}

	/// <summary>
	/// This code is not framerate-independent:
	/// 
	/// target.position += velocity;
	/// velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 9f);
	/// 
	/// But this code is:
	/// 
	/// target.position += NGUIMath.SpringDampen(ref velocity, 9f, Time.deltaTime);
	/// </summary>

	static public Vector3 SpringDampen (ref Vector3 velocity, float strength, float deltaTime)
	{
		if (deltaTime > 1f) deltaTime = 1f;
		float dampeningFactor = 1f - strength * 0.001f;
		int ms = Mathf.RoundToInt(deltaTime * 1000f);
		float totalDampening = Mathf.Pow(dampeningFactor, ms);
		Vector3 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
		velocity = velocity * totalDampening;
		return vTotal * 0.06f;
	}

	/// <summary>
	/// Same as the Vector3 version, it's a framerate-independent Lerp.
	/// </summary>

	static public Vector2 SpringDampen (ref Vector2 velocity, float strength, float deltaTime)
	{
		if (deltaTime > 1f) deltaTime = 1f;
		float dampeningFactor = 1f - strength * 0.001f;
		int ms = Mathf.RoundToInt(deltaTime * 1000f);
		float totalDampening = Mathf.Pow(dampeningFactor, ms);
		Vector2 vTotal = velocity * ((totalDampening - 1f) / Mathf.Log(dampeningFactor));
		velocity = velocity * totalDampening;
		return vTotal * 0.06f;
	}

	/// <summary>
	/// Calculate how much to interpolate by.
	/// </summary>

	static public float SpringLerp (float strength, float deltaTime)
	{
		if (deltaTime > 1f) deltaTime = 1f;
		int ms = Mathf.RoundToInt(deltaTime * 1000f);
		deltaTime = 0.001f * strength;
		float cumulative = 0f;
		for (int i = 0; i < ms; ++i) cumulative = Mathf.Lerp(cumulative, 1f, deltaTime);
		return cumulative;
	}

	/// <summary>
	/// Mathf.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
	/// </summary>

	static public float SpringLerp (float from, float to, float strength, float deltaTime)
	{
		if (deltaTime > 1f) deltaTime = 1f;
		int ms = Mathf.RoundToInt(deltaTime * 1000f);
		deltaTime = 0.001f * strength;
		for (int i = 0; i < ms; ++i) from = Mathf.Lerp(from, to, deltaTime);
		return from;
	}

	/// <summary>
	/// Vector2.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
	/// </summary>

	static public Vector2 SpringLerp (Vector2 from, Vector2 to, float strength, float deltaTime)
	{
		return Vector2.Lerp(from, to, SpringLerp(strength, deltaTime));
	}

	/// <summary>
	/// Vector3.Lerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
	/// </summary>

	static public Vector3 SpringLerp (Vector3 from, Vector3 to, float strength, float deltaTime)
	{
		return Vector3.Lerp(from, to, SpringLerp(strength, deltaTime));
	}

	/// <summary>
	/// Quaternion.Slerp(from, to, Time.deltaTime * strength) is not framerate-independent. This function is.
	/// </summary>

	static public Quaternion SpringLerp (Quaternion from, Quaternion to, float strength, float deltaTime)
	{
		return Quaternion.Slerp(from, to, SpringLerp(strength, deltaTime));
	}

	/// <summary>
	/// Since there is no Mathf.RotateTowards...
	/// </summary>

	static public float RotateTowards (float from, float to, float maxAngle)
	{
		float diff = WrapAngle(to - from);
		if (Mathf.Abs(diff) > maxAngle) diff = maxAngle * Mathf.Sign(diff);
		return from + diff;
	}

	/// <summary>
	/// Determine the distance from the specified point to the line segment.
	/// </summary>

	static float DistancePointToLineSegment (Vector2 point, Vector2 a, Vector2 b)
	{
		float l2 = (b - a).sqrMagnitude;
		if (l2 == 0f) return (point - a).magnitude;
		float t = Vector2.Dot(point - a, b - a) / l2;
		if (t < 0f) return (point - a).magnitude;
		else if (t > 1f) return (point - b).magnitude;
		Vector2 projection = a + t * (b - a);
		return (point - projection).magnitude;
	}

	/// <summary>
	/// Determine the distance from the mouse position to the screen space rectangle specified by the 4 points.
	/// </summary>

	static public float DistanceToRectangle (Vector2[] screenPoints, Vector2 mousePos)
	{
		bool oddNodes = false;
		int j = 4;

		for (int i = 0; i < 5; i++)
		{
			Vector3 v0 = screenPoints[NGUIMath.RepeatIndex(i, 4)];
			Vector3 v1 = screenPoints[NGUIMath.RepeatIndex(j, 4)];

			if ((v0.y > mousePos.y) != (v1.y > mousePos.y))
			{
				if (mousePos.x < (v1.x - v0.x) * (mousePos.y - v0.y) / (v1.y - v0.y) + v0.x)
				{
					oddNodes = !oddNodes;
				}
			}
			j = i;
		}

		if (!oddNodes)
		{
			float dist, closestDist = -1f;

			for (int i = 0; i < 4; i++)
			{
				Vector3 v0 = screenPoints[i];
				Vector3 v1 = screenPoints[NGUIMath.RepeatIndex(i + 1, 4)];

				dist = DistancePointToLineSegment(mousePos, v0, v1);

				if (dist < closestDist || closestDist < 0f) closestDist = dist;
			}
			return closestDist;
		}
		else return 0f;
	}

	/// <summary>
	/// Determine the distance from the mouse position to the world rectangle specified by the 4 points.
	/// </summary>

	static public float DistanceToRectangle (Vector3[] worldPoints, Vector2 mousePos, Camera cam)
	{
		Vector2[] screenPoints = new Vector2[4];
		for (int i = 0; i < 4; ++i)
			screenPoints[i] = cam.WorldToScreenPoint(worldPoints[i]);
		return DistanceToRectangle(screenPoints, mousePos);
	}

	/// <summary>
	/// Helper function that converts the widget's pivot enum into a 0-1 range vector.
	/// </summary>

	static public Vector2 GetPivotOffset (UIWidget.Pivot pv)
	{
		Vector2 v = Vector2.zero;

		if (pv == UIWidget.Pivot.Top || pv == UIWidget.Pivot.Center || pv == UIWidget.Pivot.Bottom) v.x = 0.5f;
		else if (pv == UIWidget.Pivot.TopRight || pv == UIWidget.Pivot.Right || pv == UIWidget.Pivot.BottomRight) v.x = 1f;
		else v.x = 0f;

		if (pv == UIWidget.Pivot.Left || pv == UIWidget.Pivot.Center || pv == UIWidget.Pivot.Right) v.y = 0.5f;
		else if (pv == UIWidget.Pivot.TopLeft || pv == UIWidget.Pivot.Top || pv == UIWidget.Pivot.TopRight) v.y = 1f;
		else v.y = 0f;

		return v;
	}

	/// <summary>
	/// Helper function that converts the pivot offset to a pivot point.
	/// </summary>

	static public UIWidget.Pivot GetPivot (Vector2 offset)
	{
		if (offset.x == 0f)
		{
			if (offset.y == 0f) return UIWidget.Pivot.BottomLeft;
			if (offset.y == 1f) return UIWidget.Pivot.TopLeft;
			return UIWidget.Pivot.Left;
		}
		else if (offset.x == 1f)
		{
			if (offset.y == 0f) return UIWidget.Pivot.BottomRight;
			if (offset.y == 1f) return UIWidget.Pivot.TopRight;
			return UIWidget.Pivot.Right;
		}
		else
		{
			if (offset.y == 0f) return UIWidget.Pivot.Bottom;
			if (offset.y == 1f) return UIWidget.Pivot.Top;
			return UIWidget.Pivot.Center;
		}
	}

	/// <summary>
	/// Adjust the widget's position using the specified local delta coordinates.
	/// </summary>

	static public void MoveWidget (UIRect w, float x, float y) { MoveRect(w, x, y); }

	/// <summary>
	/// Adjust the rectangle's position using the specified local delta coordinates.
	/// </summary>

	static public void MoveRect (UIRect rect, float x, float y)
	{
		int ix = Mathf.FloorToInt(x + 0.5f);
		int iy = Mathf.FloorToInt(y + 0.5f);

		Transform t = rect.cachedTransform;
		t.localPosition += new Vector3(ix, iy);
		int anchorCount = 0;

		if (rect.leftAnchor.target)
		{
			++anchorCount;
			rect.leftAnchor.absolute += ix;
		}

		if (rect.rightAnchor.target)
		{
			++anchorCount;
			rect.rightAnchor.absolute += ix;
		}

		if (rect.bottomAnchor.target)
		{
			++anchorCount;
			rect.bottomAnchor.absolute += iy;
		}

		if (rect.topAnchor.target)
		{
			++anchorCount;
			rect.topAnchor.absolute += iy;
		}

#if UNITY_EDITOR
		NGUITools.SetDirty(rect);
#endif

		// If all sides were anchored, we're done
		if (anchorCount != 0) rect.UpdateAnchors();
	}

	/// <summary>
	/// Given the specified dragged pivot point, adjust the widget's dimensions.
	/// </summary>

	static public void ResizeWidget (UIWidget w, UIWidget.Pivot pivot, float x, float y, int minWidth, int minHeight)
	{
		ResizeWidget(w, pivot, x, y, 2, 2, 100000, 100000);
	}

	/// <summary>
	/// Given the specified dragged pivot point, adjust the widget's dimensions.
	/// </summary>

	static public void ResizeWidget (UIWidget w, UIWidget.Pivot pivot, float x, float y, int minWidth, int minHeight, int maxWidth, int maxHeight)
	{
		if (pivot == UIWidget.Pivot.Center)
		{
			int diffX = Mathf.RoundToInt(x - w.width);
			int diffY = Mathf.RoundToInt(y - w.height);

			diffX = diffX - (diffX & 1);
			diffY = diffY - (diffY & 1);

			if ((diffX | diffY) != 0)
			{
				diffX >>= 1;
				diffY >>= 1;
				AdjustWidget(w, -diffX, -diffY, diffX, diffY, minWidth, minHeight);
			}
			return;
		}

		Vector3 v = new Vector3(x, y);
		v = Quaternion.Inverse(w.cachedTransform.localRotation) * v;

		switch (pivot)
		{
			case UIWidget.Pivot.BottomLeft:
			AdjustWidget(w, v.x, v.y, 0, 0, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.Left:
			AdjustWidget(w, v.x, 0, 0, 0, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.TopLeft:
			AdjustWidget(w, v.x, 0, 0, v.y, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.Top:
			AdjustWidget(w, 0, 0, 0, v.y, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.TopRight:
			AdjustWidget(w, 0, 0, v.x, v.y, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.Right:
			AdjustWidget(w, 0, 0, v.x, 0, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.BottomRight:
			AdjustWidget(w, 0, v.y, v.x, 0, minWidth, minHeight, maxWidth, maxHeight);
			break;

			case UIWidget.Pivot.Bottom:
			AdjustWidget(w, 0, v.y, 0, 0, minWidth, minHeight, maxWidth, maxHeight);
			break;
		}
	}

	/// <summary>
	/// Adjust the widget's rectangle based on the specified modifier values.
	/// </summary>

	static public void AdjustWidget (UIWidget w, float left, float bottom, float right, float top)
	{
		AdjustWidget(w, left, bottom, right, top, 2, 2, 100000, 100000);
	}

	/// <summary>
	/// Adjust the widget's rectangle based on the specified modifier values.
	/// </summary>

	static public void AdjustWidget (UIWidget w, float left, float bottom, float right, float top, int minWidth, int minHeight)
	{
		AdjustWidget(w, left, bottom, right, top, minWidth, minHeight, 100000, 100000);
	}

	/// <summary>
	/// Adjust the widget's rectangle based on the specified modifier values.
	/// </summary>

	static public void AdjustWidget (UIWidget w, float left, float bottom, float right, float top,
		int minWidth, int minHeight, int maxWidth, int maxHeight)
	{
		Vector2 piv = w.pivotOffset;
		Transform t = w.cachedTransform;
		Quaternion rot = t.localRotation;

		// We should be working with whole integers
		int iLeft = Mathf.FloorToInt(left + 0.5f);
		int iBottom = Mathf.FloorToInt(bottom + 0.5f);
		int iRight = Mathf.FloorToInt(right + 0.5f);
		int iTop = Mathf.FloorToInt(top + 0.5f);

		// Centered pivot should mean having to perform even number adjustments
		if (piv.x == 0.5f && (iLeft == 0 || iRight == 0))
		{
			iLeft = ((iLeft >> 1) << 1);
			iRight = ((iRight >> 1) << 1);
		}

		if (piv.y == 0.5f && (iBottom == 0 || iTop == 0))
		{
			iBottom = ((iBottom >> 1) << 1);
			iTop = ((iTop >> 1) << 1);
		}

		// The widget's position (pivot point) uses a different coordinate system than
		// other corners. This is a source of major PITA, and results in a lot of extra math.
		Vector3 rotatedTL = rot * new Vector3(iLeft, iTop);
		Vector3 rotatedTR = rot * new Vector3(iRight, iTop);
		Vector3 rotatedBL = rot * new Vector3(iLeft, iBottom);
		Vector3 rotatedBR = rot * new Vector3(iRight, iBottom);
		Vector3 rotatedL = rot * new Vector3(iLeft, 0f);
		Vector3 rotatedR = rot * new Vector3(iRight, 0f);
		Vector3 rotatedT = rot * new Vector3(0f, iTop);
		Vector3 rotatedB = rot * new Vector3(0f, iBottom);

		Vector3 offset = Vector3.zero;

		if (piv.x == 0f && piv.y == 1f)
		{
			offset.x = rotatedTL.x;
			offset.y = rotatedTL.y;
		}
		else if (piv.x == 1f && piv.y == 0f)
		{
			offset.x = rotatedBR.x;
			offset.y = rotatedBR.y;
		}
		else if (piv.x == 0f && piv.y == 0f)
		{
			offset.x = rotatedBL.x;
			offset.y = rotatedBL.y;
		}
		else if (piv.x == 1f && piv.y == 1f)
		{
			offset.x = rotatedTR.x;
			offset.y = rotatedTR.y;
		}
		else if (piv.x == 0f && piv.y == 0.5f)
		{
			offset.x = rotatedL.x + (rotatedT.x + rotatedB.x) * 0.5f;
			offset.y = rotatedL.y + (rotatedT.y + rotatedB.y) * 0.5f;
		}
		else if (piv.x == 1f && piv.y == 0.5f)
		{
			offset.x = rotatedR.x + (rotatedT.x + rotatedB.x) * 0.5f;
			offset.y = rotatedR.y + (rotatedT.y + rotatedB.y) * 0.5f;
		}
		else if (piv.x == 0.5f && piv.y == 1f)
		{
			offset.x = rotatedT.x + (rotatedL.x + rotatedR.x) * 0.5f;
			offset.y = rotatedT.y + (rotatedL.y + rotatedR.y) * 0.5f;
		}
		else if (piv.x == 0.5f && piv.y == 0f)
		{
			offset.x = rotatedB.x + (rotatedL.x + rotatedR.x) * 0.5f;
			offset.y = rotatedB.y + (rotatedL.y + rotatedR.y) * 0.5f;
		}
		else if (piv.x == 0.5f && piv.y == 0.5f)
		{
			offset.x = (rotatedL.x + rotatedR.x + rotatedT.x + rotatedB.x) * 0.5f;
			offset.y = (rotatedT.y + rotatedB.y + rotatedL.y + rotatedR.y) * 0.5f;
		}

		minWidth = Mathf.Max(minWidth, w.minWidth);
		minHeight = Mathf.Max(minHeight, w.minHeight);

		// Calculate the widget's width and height after the requested adjustments
		int finalWidth = w.width + iRight - iLeft;
		int finalHeight = w.height + iTop - iBottom;

		// Now it's time to constrain the width and height so that they can't go below min values
		Vector3 constraint = Vector3.zero;

		int limitWidth = finalWidth;
		if (finalWidth < minWidth) limitWidth = minWidth;
		else if (finalWidth > maxWidth) limitWidth = maxWidth;

		if (finalWidth != limitWidth)
		{
			if (iLeft != 0) constraint.x -= Mathf.Lerp(limitWidth - finalWidth, 0f, piv.x);
			else constraint.x += Mathf.Lerp(0f, limitWidth - finalWidth, piv.x);
			finalWidth = limitWidth;
		}

		int limitHeight = finalHeight;
		if (finalHeight < minHeight) limitHeight = minHeight;
		else if (finalHeight > maxHeight) limitHeight = maxHeight;

		if (finalHeight != limitHeight)
		{
			if (iBottom != 0) constraint.y -= Mathf.Lerp(limitHeight - finalHeight, 0f, piv.y);
			else constraint.y += Mathf.Lerp(0f, limitHeight - finalHeight, piv.y);
			finalHeight = limitHeight;
		}

		// Centered pivot requires power-of-two dimensions
		if (piv.x == 0.5f) finalWidth  = ((finalWidth  >> 1) << 1);
		if (piv.y == 0.5f) finalHeight = ((finalHeight >> 1) << 1);

		// Update the position, width and height
		Vector3 pos = t.localPosition + offset + rot * constraint;
		t.localPosition = pos;
		w.SetDimensions(finalWidth, finalHeight);

		// If the widget is anchored, we should update the anchors as well
		if (w.isAnchored)
		{
			t = t.parent;
			float x = pos.x - piv.x * finalWidth;
			float y = pos.y - piv.y * finalHeight;

			if (w.leftAnchor.target) w.leftAnchor.SetHorizontal(t, x);
			if (w.rightAnchor.target) w.rightAnchor.SetHorizontal(t, x + finalWidth);
			if (w.bottomAnchor.target) w.bottomAnchor.SetVertical(t, y);
			if (w.topAnchor.target) w.topAnchor.SetVertical(t, y + finalHeight);
		}

#if UNITY_EDITOR
		NGUITools.SetDirty(w);
#endif
	}

	/// <summary>
	/// Adjust the specified value by DPI: height * 96 / DPI.
	/// This will result in in a smaller value returned for higher pixel density devices.
	/// </summary>

	static public int AdjustByDPI (float height)
	{
		float dpi = Screen.dpi;

		RuntimePlatform platform = Application.platform;

		if (dpi == 0f)
		{
			dpi = (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer) ? 160f : 96f;
#if UNITY_BLACKBERRY
			if (platform == RuntimePlatform.BB10Player) dpi = 160f;
#elif UNITY_WP8 || UNITY_WP_8_1
			if (platform == RuntimePlatform.WP8Player) dpi = 160f;
#endif
		}

		int h = Mathf.RoundToInt(height * (96f / dpi));
		if ((h & 1) == 1) ++h;
		return h;
	}

	/// <summary>
	/// Convert the specified position, making it relative to the specified object.
	/// </summary>

	static public Vector2 ScreenToPixels (Vector2 pos, Transform relativeTo)
	{
		int layer = relativeTo.gameObject.layer;
		Camera cam = NGUITools.FindCameraForLayer(layer);

		if (cam == null)
		{
			Debug.LogWarning("No camera found for layer " + layer);
			return pos;
		}

		Vector3 wp = cam.ScreenToWorldPoint(pos);
		return relativeTo.InverseTransformPoint(wp);
	}

	/// <summary>
	/// Convert the specified position, making it relative to the specified object's parent.
	/// Useful if you plan on positioning the widget using the specified value (think mouse cursor).
	/// </summary>

	static public Vector2 ScreenToParentPixels (Vector2 pos, Transform relativeTo)
	{
		int layer = relativeTo.gameObject.layer;
		if (relativeTo.parent != null)
			relativeTo = relativeTo.parent;

		Camera cam = NGUITools.FindCameraForLayer(layer);

		if (cam == null)
		{
			Debug.LogWarning("No camera found for layer " + layer);
			return pos;
		}

		Vector3 wp = cam.ScreenToWorldPoint(pos);
		return (relativeTo != null) ? relativeTo.InverseTransformPoint(wp) : wp;
	}

	/// <summary>
	/// Convert the specified world point from one camera's world space to another, then make it relative to the specified transform.
	/// You should use this function if you want to position a widget using some 3D point in space.
	/// Pass your main camera for the "worldCam", and your UI camera for "uiCam", then the widget's transform for "relativeTo".
	/// You can then assign the widget's localPosition to the returned value.
	/// </summary>

	static public Vector3 WorldToLocalPoint (Vector3 worldPos, Camera worldCam, Camera uiCam, Transform relativeTo)
	{
		worldPos = worldCam.WorldToViewportPoint(worldPos);
		worldPos = uiCam.ViewportToWorldPoint(worldPos);
		if (relativeTo == null) return worldPos;
		relativeTo = relativeTo.parent;
		if (relativeTo == null) return worldPos;
		return relativeTo.InverseTransformPoint(worldPos);
	}

	/// <summary>
	/// Helper function that can set the transform's position to be at the specified world position.
	/// Ideal usage: positioning a UI element to be directly over a 3D point in space.
	/// </summary>
	/// <param name="worldPos">World position, visible by the worldCam</param>
	/// <param name="worldCam">Camera that is able to see the worldPos</param>
	/// <param name="myCam">Camera that is able to see the transform this function is called on</param>

	static public void OverlayPosition (this Transform trans, Vector3 worldPos, Camera worldCam, Camera myCam)
	{
		worldPos = worldCam.WorldToViewportPoint(worldPos);
		worldPos = myCam.ViewportToWorldPoint(worldPos);
		Transform parent = trans.parent;
		trans.localPosition = (parent != null) ? parent.InverseTransformPoint(worldPos) : worldPos;
	}

	/// <summary>
	/// Helper function that can set the transform's position to be at the specified world position.
	/// Ideal usage: positioning a UI element to be directly over a 3D point in space.
	/// </summary>
	/// <param name="worldPos">World position, visible by the worldCam</param>
	/// <param name="worldCam">Camera that is able to see the worldPos</param>

	static public void OverlayPosition (this Transform trans, Vector3 worldPos, Camera worldCam)
	{
		Camera myCam = NGUITools.FindCameraForLayer(trans.gameObject.layer);
		if (myCam != null) trans.OverlayPosition(worldPos, worldCam, myCam);
	}

	/// <summary>
	/// Helper function that can set the transform's position to be over the specified target transform.
	/// Ideal usage: positioning a UI element to be directly over a 3D object in space.
	/// </summary>
	/// <param name="target">Target over which the transform should be positioned</param>

	static public void OverlayPosition (this Transform trans, Transform target)
	{
		Camera myCam = NGUITools.FindCameraForLayer(trans.gameObject.layer);
		Camera worldCam = NGUITools.FindCameraForLayer(target.gameObject.layer);
		if (myCam != null && worldCam != null) trans.OverlayPosition(target.position, worldCam, myCam);
	}
}
