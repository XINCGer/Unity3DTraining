//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Editor class used to view UIRects.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIRect), true)]
public class UIRectEditor : Editor
{
	static public UIRectEditor instance;

	static protected string[] PrefixName = new string[] { "Left", "Right", "Bottom", "Top" };
	static protected string[] FieldName = new string[] { "leftAnchor", "rightAnchor", "bottomAnchor", "topAnchor" };
	static protected string[] HorizontalList = new string[] { "Target's Left", "Target's Center", "Target's Right", "Custom", "Set to Current Position" };
	static protected string[] VerticalList = new string[] { "Target's Bottom", "Target's Center", "Target's Top", "Custom", "Set to Current Position" };
	static protected bool[] IsHorizontal = new bool[] { true, true, false, false };

	protected enum AnchorType
	{
		None,
		Unified,
		Advanced,
	}

	protected AnchorType mAnchorType = AnchorType.None;
	protected Transform[] mTarget = new Transform[4];
	protected bool[] mCustom = new bool[] { false, false, false, false };

	/// <summary>
	/// Whether the specified relative offset is a common value (0, 0.5, or 1)
	/// </summary>

	static protected bool IsCommon (float relative) { return (relative == 0f || relative == 0.5f || relative == 1f); }

	/// <summary>
	/// Returns 'true' if the specified serialized property reference is a UIRect.
	/// </summary>

	static protected bool IsRect (SerializedProperty sp)
	{
		if (sp.hasMultipleDifferentValues) return true;
		return (GetRect(sp) != null);
	}

	/// <summary>
	/// Pass something like leftAnchor.target to get its rectangle reference.
	/// </summary>

	static protected UIRect GetRect (SerializedProperty sp)
	{
		Transform target = sp.objectReferenceValue as Transform;
		if (target == null) return null;
		return target.GetComponent<UIRect>();
	}

	/// <summary>
	/// Pass something like leftAnchor.target to get its rectangle reference.
	/// </summary>

	static protected Camera GetCamera (SerializedProperty sp)
	{
		Transform target = sp.objectReferenceValue as Transform;
		if (target == null) return null;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		return target.camera;
#else
		return target.GetComponent<Camera>();
#endif
	}

	/// <summary>
	/// Determine the initial anchor type.
	/// </summary>

	protected virtual void OnEnable ()
	{
		instance = this;

		if (serializedObject.isEditingMultipleObjects)
		{
			mAnchorType = AnchorType.Advanced;
		}
		else ReEvaluateAnchorType();
	}

	/// <summary>
	/// Clear the instance reference.
	/// </summary>

	protected virtual void OnDisable () { instance = null; }

	/// <summary>
	/// Manually re-evaluate the current anchor type.
	/// </summary>

	protected void ReEvaluateAnchorType ()
	{
		UIRect rect = target as UIRect;

		if (rect.leftAnchor.target == rect.rightAnchor.target &&
			rect.leftAnchor.target == rect.bottomAnchor.target &&
			rect.leftAnchor.target == rect.topAnchor.target)
		{
			if (rect.leftAnchor.target == null)
			{
				mAnchorType = AnchorType.None;
			}
			else
			{
				mAnchorType = AnchorType.Unified;
			}
		}
		else mAnchorType = AnchorType.Advanced;
	}

	/// <summary>
	/// Draw the inspector properties.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);
		EditorGUILayout.Space();

		serializedObject.Update();

		EditorGUI.BeginDisabledGroup(!ShouldDrawProperties());
		DrawCustomProperties();
		EditorGUI.EndDisabledGroup();
		DrawFinalProperties();

		serializedObject.ApplyModifiedProperties();
	}

	protected virtual bool ShouldDrawProperties () { return true; }
	protected virtual void DrawCustomProperties () { }

	/// <summary>
	/// Draw the "Anchors" property block.
	/// </summary>

	protected virtual void DrawFinalProperties () { if (!NGUISettings.unifiedTransform) DrawAnchorTransform(); }
	protected virtual void OnDrawFinalProperties () { }

	public void DrawAnchorTransform ()
	{
		if (NGUIEditorTools.DrawHeader("Anchors"))
		{
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.SetLabelWidth(NGUISettings.minimalisticLook ? 69f : 62f);

			EditorGUI.BeginDisabledGroup(!((target as UIRect).canBeAnchored));
			GUILayout.BeginHorizontal();
			AnchorType type = (AnchorType)EditorGUILayout.EnumPopup("Type", mAnchorType);
			NGUIEditorTools.DrawPadding();
			GUILayout.EndHorizontal();

			SerializedProperty[] tg = new SerializedProperty[4];
			for (int i = 0; i < 4; ++i) tg[i] = serializedObject.FindProperty(FieldName[i] + ".target");

			if (mAnchorType == AnchorType.None && type != AnchorType.None)
			{
				if (type == AnchorType.Unified)
				{
					if (mTarget[0] == null && mTarget[1] == null && mTarget[2] == null && mTarget[3] == null)
					{
						UIRect rect = target as UIRect;
						UIRect parent = NGUITools.FindInParents<UIRect>(rect.cachedTransform.parent);

						if (parent != null)
							for (int i = 0; i < 4; ++i)
								mTarget[i] = parent.cachedTransform;
					}
				}

				for (int i = 0; i < 4; ++i)
				{
					tg[i].objectReferenceValue = mTarget[i];
					mTarget[i] = null;
				}
				UpdateAnchors(true);
			}

			if (type != AnchorType.None)
			{
				NGUIEditorTools.DrawPaddedProperty("Execute", serializedObject, "updateAnchors");
			}

			if (type == AnchorType.Advanced)
			{
				DrawAnchor(0, true);
				DrawAnchor(1, true);
				DrawAnchor(2, true);
				DrawAnchor(3, true);
			}
			else if (type == AnchorType.Unified)
			{
				DrawSingleAnchorSelection();

				DrawAnchor(0, false);
				DrawAnchor(1, false);
				DrawAnchor(2, false);
				DrawAnchor(3, false);
			}
			else if (type == AnchorType.None && mAnchorType != type)
			{
				// Save values to make it easy to "go back"
				for (int i = 0; i < 4; ++i)
				{
					mTarget[i] = tg[i].objectReferenceValue as Transform;
					tg[i].objectReferenceValue = null;
				}

				serializedObject.FindProperty("leftAnchor.relative").floatValue = 0f;
				serializedObject.FindProperty("bottomAnchor.relative").floatValue = 0f;
				serializedObject.FindProperty("rightAnchor.relative").floatValue = 1f;
				serializedObject.FindProperty("topAnchor.relative").floatValue = 1f;
			}

			mAnchorType = type;
			OnDrawFinalProperties();
			EditorGUI.EndDisabledGroup();
			NGUIEditorTools.EndContents();
		}
	}

	/// <summary>
	/// Draw a selection for a single target (one target sets all 4 sides)
	/// </summary>

	protected SerializedProperty DrawSingleAnchorSelection ()
	{
		SerializedProperty sp = serializedObject.FindProperty("leftAnchor.target");
		Object before = sp.objectReferenceValue;

		GUILayout.Space(3f);
		NGUIEditorTools.DrawProperty("Target", sp, false);

		Object after = sp.objectReferenceValue;
		serializedObject.FindProperty("rightAnchor.target").objectReferenceValue = after;
		serializedObject.FindProperty("bottomAnchor.target").objectReferenceValue = after;
		serializedObject.FindProperty("topAnchor.target").objectReferenceValue = after;

		if (after != null || sp.hasMultipleDifferentValues)
		{
			if (before != after && after != null)
				UpdateAnchors(true);
		}
		return sp;
	}

	/// <summary>
	/// Helper function that draws the suffix after the relative fields.
	/// </summary>

	protected void DrawAnchor (int index, bool targetSelection)
	{
		//if (targetSelection) GUILayout.Space(3f);

		//NGUIEditorTools.SetLabelWidth(16f);
		GUILayout.BeginHorizontal();
		GUILayout.Label(PrefixName[index], GUILayout.Width(NGUISettings.minimalisticLook ? 65f : 56f));

		UIRect myRect = serializedObject.targetObject as UIRect;
		string name = FieldName[index];

		SerializedProperty tar = serializedObject.FindProperty(name + ".target");
		SerializedProperty rel = serializedObject.FindProperty(name + ".relative");
		SerializedProperty abs = serializedObject.FindProperty(name + ".absolute");

		if (targetSelection)
		{
			Object before = tar.objectReferenceValue;
			NGUIEditorTools.DrawProperty("", tar, false, GUILayout.MinWidth(20f));
			Object after = tar.objectReferenceValue;

			if (after != null || tar.hasMultipleDifferentValues)
			{
				if (before != after && after != null)
					UpdateAnchor(index, true);
			}

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label(" ", GUILayout.Width(NGUISettings.minimalisticLook ? 65f : 56f));
		}

		UIRect targetRect = GetRect(tar);
		Camera targetCam = GetCamera(tar);
		float relative = rel.floatValue;
		bool isCommon = (targetRect == null && targetCam == null) || IsCommon(relative);
		int previousOrigin = 1;

		if (targetRect != null || targetCam != null)
		{
			if (mCustom[index] || !isCommon) previousOrigin = 3;
			else if (relative == 0f) previousOrigin = 0;
			else if (relative == 1f) previousOrigin = 2;
		}

		// Draw the origin selection list
		EditorGUI.BeginDisabledGroup(targetRect == null && targetCam == null);
		int newOrigin = IsHorizontal[index] ?
			EditorGUILayout.Popup(previousOrigin, HorizontalList) :
			EditorGUILayout.Popup(previousOrigin, VerticalList);
		EditorGUI.EndDisabledGroup();

		// "Set to Current" choice
		if (newOrigin == 4)
		{
			newOrigin = 3;

			Vector3[] sides = targetRect.GetSides(myRect.cachedTransform);

			float f0, f1;

			if (IsHorizontal[index])
			{
				f0 = sides[0].x;
				f1 = sides[2].x;
			}
			else
			{
				f0 = sides[3].y;
				f1 = sides[1].y;
			}

			// Final position after both relative and absolute values are taken into consideration
			float final = Mathf.Floor(0.5f + Mathf.Lerp(0f, f1 - f0, rel.floatValue) + abs.intValue);

			rel.floatValue = final / (f1 - f0);
			abs.intValue = 0;

			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		mCustom[index] = (newOrigin == 3);

		// If the origin changes
		if (newOrigin != 3 && previousOrigin != newOrigin)
		{
			// Desired relative value
			if (newOrigin == 0) relative = 0f;
			else if (newOrigin == 2) relative = 1f;
			else relative = 0.5f;

			Vector3[] sides = (targetRect != null) ?
				targetRect.GetSides(myRect.cachedTransform) :
				targetCam.GetSides(myRect.cachedTransform);

			// Calculate the current position based from the bottom-left
			float f0, f1;

			if (IsHorizontal[index])
			{
				f0 = sides[0].x;
				f1 = sides[2].x;
			}
			else
			{
				f0 = sides[3].y;
				f1 = sides[1].y;
			}

			// Final position after both relative and absolute values are taken into consideration
			float final = Mathf.Floor(0.5f + Mathf.Lerp(f0, f1, rel.floatValue) + abs.intValue);

			rel.floatValue = relative;
			abs.intValue = Mathf.FloorToInt(final + 0.5f - Mathf.Lerp(f0, f1, relative));

			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		if (mCustom[index])
		{
			// Draw the relative value
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Space(64f);

			relative = rel.floatValue;
			bool isOutside01 = relative < 0f || relative > 1f;

			// Horizontal slider for relative values, for convenience
			//EditorGUI.BeginDisabledGroup(isOutside01);
			{
				GUILayout.Space(10f);
				float val = GUILayout.HorizontalSlider(relative, 0f, 1f);

				NGUIEditorTools.DrawProperty("", rel, false, GUILayout.Width(40f));

				if (!isOutside01 && val != relative)
				{
					Vector3[] sides = (targetRect != null) ?
						targetRect.GetSides(myRect.cachedTransform) :
						targetCam.GetSides(myRect.cachedTransform);

					// Calculate the current position based from the bottom-left
					float f0, f1;

					if (IsHorizontal[index])
					{
						f0 = sides[0].x;
						f1 = sides[2].x;
					}
					else
					{
						f0 = sides[3].y;
						f1 = sides[1].y;
					}

					float size = (f1 - f0);
					int intVal = Mathf.FloorToInt(val * size + 0.5f);
					//intVal = ((intVal >> 1) << 1);
					rel.floatValue = (size > 0f) ? intVal / size : 0.5f;
				}
			}
			//EditorGUI.EndDisabledGroup();
		}

		// Draw the absolute value
		NGUIEditorTools.SetLabelWidth(16f);
		NGUIEditorTools.DrawProperty("+", abs, false, GUILayout.Width(60f));
		
		GUILayout.EndHorizontal();
		NGUIEditorTools.SetLabelWidth(NGUISettings.minimalisticLook ? 69f : 62f);
	}

	/// <summary>
	/// Convenience function that switches the anchor mode and ensures that dimensions are kept intact.
	/// </summary>

	protected void UpdateAnchors (bool resetRelative)
	{
		serializedObject.ApplyModifiedProperties();

		Object[] objs = serializedObject.targetObjects;

		for (int i = 0; i < objs.Length; ++i)
		{
			UIRect rect = objs[i] as UIRect;

			if (rect)
			{
				UpdateHorizontalAnchor(rect, rect.leftAnchor, resetRelative);
				UpdateHorizontalAnchor(rect, rect.rightAnchor, resetRelative);
				UpdateVerticalAnchor(rect, rect.bottomAnchor, resetRelative);
				UpdateVerticalAnchor(rect, rect.topAnchor, resetRelative);
				
				NGUITools.SetDirty(rect);
			}
		}
		serializedObject.Update();
	}

	/// <summary>
	/// Convenience function that switches the anchor mode and ensures that dimensions are kept intact.
	/// </summary>

	protected void UpdateAnchor (int index, bool resetRelative)
	{
		serializedObject.ApplyModifiedProperties();

		Object[] objs = serializedObject.targetObjects;

		for (int i = 0; i < objs.Length; ++i)
		{
			UIRect rect = objs[i] as UIRect;

			if (rect)
			{
				if (index == 0) UpdateHorizontalAnchor(rect, rect.leftAnchor, resetRelative);
				if (index == 1) UpdateHorizontalAnchor(rect, rect.rightAnchor, resetRelative);
				if (index == 2) UpdateVerticalAnchor(rect, rect.bottomAnchor, resetRelative);
				if (index == 3) UpdateVerticalAnchor(rect, rect.topAnchor, resetRelative);

				NGUITools.SetDirty(rect);
			}
		}
		serializedObject.Update();
	}

	/// <summary>
	/// Convenience function that switches the anchor mode and ensures that dimensions are kept intact.
	/// </summary>

	static public void UpdateHorizontalAnchor (UIRect r, UIRect.AnchorPoint anchor, bool resetRelative)
	{
		// Update the target
		if (anchor.target == null) return;

		// Update the rect
		anchor.rect = anchor.target.GetComponent<UIRect>();

		// Continue only if we have a parent to work with
		Transform parent = r.cachedTransform.parent;
		if (parent == null) return;

		bool inverted = (anchor == r.rightAnchor);
		int i0 = inverted ? 2 : 0;
		int i1 = inverted ? 3 : 1;

		// Calculate the left side
		Vector3[] myCorners = r.worldCorners;
		Vector3 localPos = parent.InverseTransformPoint(Vector3.Lerp(myCorners[i0], myCorners[i1], 0.5f));

		if (anchor.rect != null)
		{
			// Anchored to a rectangle -- must anchor to the same side
			Vector3[] targetCorners = anchor.rect.worldCorners;

			// We want to choose the side with the shortest offset
			Vector3 side0 = parent.InverseTransformPoint(Vector3.Lerp(targetCorners[0], targetCorners[1], 0.5f));
			Vector3 side1 = parent.InverseTransformPoint(Vector3.Lerp(targetCorners[2], targetCorners[3], 0.5f));

			float val0 = localPos.x - side0.x;
			float val2 = localPos.x - side1.x;

			if (resetRelative)
			{
				float val1 = localPos.x - Vector3.Lerp(side0, side1, 0.5f).x;
				anchor.SetToNearest(val0, val1, val2);
			}
			else
			{
				float val = localPos.x - Vector3.Lerp(side0, side1, anchor.relative).x;
				anchor.Set(anchor.relative, val);
			}
		}
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		else if (anchor.target.camera != null)
		{
			Vector3[] sides = anchor.target.camera.GetSides(parent);
#else
		else if (anchor.target.GetComponent<Camera>() != null)
		{
			Vector3[] sides = anchor.target.GetComponent<Camera>().GetSides(parent);
#endif
			Vector3 side0 = sides[0];
			Vector3 side1 = sides[2];

			float val0 = localPos.x - side0.x;
			float val2 = localPos.x - side1.x;

			if (resetRelative)
			{
				float val1 = localPos.x - Vector3.Lerp(side0, side1, 0.5f).x;
				anchor.SetToNearest(val0, val1, val2);
			}
			else
			{
				float val = localPos.x - Vector3.Lerp(side0, side1, anchor.relative).x;
				anchor.Set(anchor.relative, val);
			}
		}
		else
		{
			// Anchored to a simple transform
			Vector3 remotePos = anchor.target.position;
			if (anchor.targetCam != null) remotePos = anchor.targetCam.WorldToViewportPoint(remotePos);
			if (r.anchorCamera != null) remotePos = r.anchorCamera.ViewportToWorldPoint(remotePos);
			remotePos = parent.InverseTransformPoint(remotePos);
			anchor.absolute = Mathf.FloorToInt(localPos.x - remotePos.x + 0.5f);
			anchor.relative = inverted ? 1f : 0f;
		}
	}

	/// <summary>
	/// Convenience function that switches the anchor mode and ensures that dimensions are kept intact.
	/// </summary>

	static public void UpdateVerticalAnchor (UIRect r, UIRect.AnchorPoint anchor, bool resetRelative)
	{
		// Update the target
		if (anchor.target == null) return;

		// Update the rect
		anchor.rect = anchor.target.GetComponent<UIRect>();

		// Continue only if we have a parent to work with
		Transform parent = r.cachedTransform.parent;
		if (parent == null) return;

		bool inverted = (anchor == r.topAnchor);
		int i0 = inverted ? 1 : 0;
		int i1 = inverted ? 2 : 3;

		// Calculate the bottom side
		Vector3[] myCorners = r.worldCorners;
		Vector3 localPos = parent.InverseTransformPoint(Vector3.Lerp(myCorners[i0], myCorners[i1], 0.5f));

		if (anchor.rect != null)
		{
			// Anchored to a rectangle -- must anchor to the same side
			Vector3[] targetCorners = anchor.rect.worldCorners;

			// We want to choose the side with the shortest offset
			Vector3 side0 = parent.InverseTransformPoint(Vector3.Lerp(targetCorners[0], targetCorners[3], 0.5f));
			Vector3 side1 = parent.InverseTransformPoint(Vector3.Lerp(targetCorners[1], targetCorners[2], 0.5f));

			float val0 = localPos.y - side0.y;
			float val2 = localPos.y - side1.y;

			if (resetRelative)
			{
				float val1 = localPos.y - Vector3.Lerp(side0, side1, 0.5f).y;
				anchor.SetToNearest(val0, val1, val2);
			}
			else
			{
				float val = localPos.y - Vector3.Lerp(side0, side1, anchor.relative).y;
				anchor.Set(anchor.relative, val);
			}
		}
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		else if (anchor.target.camera != null)
		{
			Vector3[] sides = anchor.target.camera.GetSides(parent);
#else
		else if (anchor.target.GetComponent<Camera>() != null)
		{
			Vector3[] sides = anchor.target.GetComponent<Camera>().GetSides(parent);
#endif
			Vector3 side0 = sides[3];
			Vector3 side1 = sides[1];

			float val0 = localPos.y - side0.y;
			float val2 = localPos.y - side1.y;

			if (resetRelative)
			{
				float val1 = localPos.y - Vector3.Lerp(side0, side1, 0.5f).y;
				anchor.SetToNearest(val0, val1, val2);
			}
			else
			{
				float val = localPos.y - Vector3.Lerp(side0, side1, anchor.relative).y;
				anchor.Set(anchor.relative, val);
			}
		}
		else
		{
			// Anchored to a simple transform
			Vector3 remotePos = anchor.target.position;
			if (anchor.targetCam != null) remotePos = anchor.targetCam.WorldToViewportPoint(remotePos);
			if (r.anchorCamera != null) remotePos = r.anchorCamera.ViewportToWorldPoint(remotePos);
			remotePos = parent.InverseTransformPoint(remotePos);
			anchor.absolute = Mathf.FloorToInt(localPos.y - remotePos.y + 0.5f);
			anchor.relative = inverted ? 1f : 0f;
		}
	}
}
