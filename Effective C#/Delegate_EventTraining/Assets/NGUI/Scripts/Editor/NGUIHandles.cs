//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor helper class containing functions related to drawing things in the Scene View using UnityEditor.Handles.
/// </summary>

static public class NGUIHandles
{
	/// <summary>
	/// Given a plane the rectangle lies upon, convert the given screen coordinates to world coordinates.
	/// </summary>

	static public bool ScreenToWorldPoint (Plane p, Vector2 screenPos, out Vector3 worldPos)
	{
		float dist;
		Ray ray = HandleUtility.GUIPointToWorldRay(screenPos);

		if (p.Raycast(ray, out dist))
		{
			worldPos = ray.GetPoint(dist);
			return true;
		}
		worldPos = Vector3.zero;
		return false;
	}

	/// <summary>
	/// Given the widget's corners, convert the given screen coordinates to world coordinates.
	/// </summary>

	static public bool ScreenToWorldPoint (Vector3[] corners, Vector2 screenPos, out Vector3 worldPos)
	{
		Plane p = new Plane(corners[0], corners[1], corners[3]);
		return ScreenToWorldPoint(p, screenPos, out worldPos);
	}

	/// <summary>
	/// Draw width and height around the rectangle specified by the four world-space corners.
	/// </summary>

	static public void DrawSize (Vector3[] worldPos, int width, int height)
	{
		Vector2[] screenPos = new Vector2[4];

		for (int i = 0; i < 4; ++i)
			screenPos[i] = HandleUtility.WorldToGUIPoint(worldPos[i]);

		Vector2 up = (screenPos[1] - screenPos[0]).normalized;
		Vector2 right = (screenPos[3] - screenPos[0]).normalized;

		Vector2 v0 = screenPos[0] - right * 20f;
		Vector2 v1 = screenPos[1] - right * 20f;

		Vector2 v2 = screenPos[0] - up * 20f;
		Vector2 v3 = screenPos[3] - up * 20f;

		Plane p = new Plane(worldPos[0], worldPos[1], worldPos[3]);
		Color color = new Color(1f, 1f, 1f, 1f);

		DrawShadowedLine(p, v0, v1, color);
		DrawShadowedLine(p, v2, v3, color);

		DrawShadowedLine(p, v0 - right * 10f, v0 + right * 10f, color);
		DrawShadowedLine(p, v1 - right * 10f, v1 + right * 10f, color);

		DrawShadowedLine(p, v2 - up * 10f, v2 + up * 10f, color);
		DrawShadowedLine(p, v3 - up * 10f, v3 + up * 10f, color);

		if (Event.current.type == EventType.Repaint)
		{
			Handles.BeginGUI();
			DrawCenteredLabel((v2 + v3) * 0.5f, width.ToString());
			DrawCenteredLabel((v0 + v1) * 0.5f, height.ToString());
			Handles.EndGUI();
		}
	}

	/// <summary>
	/// Draw a shadowed line in the scene view.
	/// </summary>

	static public void DrawShadowedLine (Plane p, Vector2 screenPos0, Vector2 screenPos1, Color c)
	{
		Handles.color = new Color(0f, 0f, 0f, 0.5f);
		DrawLine(p, screenPos0 + Vector2.one, screenPos1 + Vector2.one);
		Handles.color = c;
		DrawLine(p, screenPos0, screenPos1);
	}

	/// <summary>
	/// Draw a shadowed line in the scene view.
	/// </summary>

	static public void DrawShadowedLine (Plane p, Vector3 worldPos0, Vector3 worldPos1, Color c)
	{
		Vector2 s0 = HandleUtility.WorldToGUIPoint(worldPos0);
		Vector2 s1 = HandleUtility.WorldToGUIPoint(worldPos1);
		DrawShadowedLine(p, s0, s1, c);
	}

	/// <summary>
	/// Draw a shadowed line in the scene view.
	/// </summary>

	static public void DrawShadowedLine (Vector3[] corners, Vector3 worldPos0, Vector3 worldPos1, Color c)
	{
		Plane p = new Plane(corners[0], corners[1], corners[3]);
		Vector2 s0 = HandleUtility.WorldToGUIPoint(worldPos0);
		Vector2 s1 = HandleUtility.WorldToGUIPoint(worldPos1);
		DrawShadowedLine(p, s0, s1, c);
	}

	/// <summary>
	/// Draw a line in the scene view.
	/// </summary>

	static public void DrawLine (Plane p, Vector2 v0, Vector2 v1)
	{
#if UNITY_3_5
		// Unity 3.5 exhibits a strange offset...
		v0.x += 1f;
		v1.x += 1f;
		v0.y -= 1f;
		v1.y -= 1f;
#endif
		Vector3 w0, w1;
		if (ScreenToWorldPoint(p, v0, out w0) && ScreenToWorldPoint(p, v1, out w1))
			Handles.DrawLine(w0, w1);
	}

	/// <summary>
	/// Draw a centered label at the specified world coordinates.
	/// </summary>

	static public void DrawCenteredLabel (Vector3 worldPos, string text) { DrawCenteredLabel(worldPos, text, 60f); }

	/// <summary>
	/// Draw a centered label at the specified world coordinates.
	/// </summary>

	static public void DrawCenteredLabel (Vector3 worldPos, string text, float width)
	{
		Vector2 screenPoint = HandleUtility.WorldToGUIPoint(worldPos);
		DrawCenteredLabel(screenPoint, text, width);
	}

	/// <summary>
	/// Draw a centered label at the specified screen coordinates.
	/// </summary>

	static public void DrawCenteredLabel (Vector2 screenPos, string text) { DrawCenteredLabel(screenPos, text, 60f); }

	/// <summary>
	/// Draw a centered label at the specified screen coordinates.
	/// It's expected that this call happens inside Handles.BeginGUI() / Handles.EndGUI().
	/// </summary>

	static public void DrawCenteredLabel (Vector2 screenPos, string text, float width)
	{
		if (Event.current.type == EventType.Repaint)
		{
			Color c = GUI.color;

			float tw = Mathf.Max(2, text.Length) * 15f;
			GUI.color = new Color(0f, 0f, 0f, 0.75f);
			GUI.Box(new Rect(screenPos.x - tw * 0.5f, screenPos.y - 10f, tw, 20f), "", "WinBtnInactiveMac");

			GUI.color = c;
#if UNITY_3_5
			GUI.Label(new Rect(screenPos.x - 30f, screenPos.y - 14f, 60f, 20f), text, "PreLabel");
#else
			GUI.Label(new Rect(screenPos.x - 30f, screenPos.y - 10f, 60f, 20f), text, "PreLabel");
#endif
		}
	}
}
