//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

/// <summary>
/// Widget containers are classes that are meant to hold more than one widget inside, but should still be easily movable using the mouse.
/// </summary>

[CanEditMultipleObjects]
[CustomEditor(typeof(UIWidgetContainer), true)]
public class UIWidgetContainerEditor : Editor
{
	static int mHash = "WidgetContainer".GetHashCode();
	
	Vector3 mStartPos = Vector3.zero;
	Vector3 mStartDrag = Vector3.zero;
	Vector2 mStartMouse = Vector2.zero;

	bool mCanDrag = false;
	bool mAllowSelection = true;
	bool mIsDragging = false;

	void OnDisable () { NGUIEditorTools.HideMoveTool(false); }

	/// <summary>
	/// Make it possible to easily drag the transform around.
	/// </summary>

	public void OnSceneGUI ()
	{
		//NGUIEditorTools.HideMoveTool(true);
		if (!UIWidget.showHandles) return;

		MonoBehaviour mb = target as MonoBehaviour;
		if (mb.GetComponent<UIWidget>() != null) return;
		if (mb.GetComponent<UIPanel>() != null) return;

		Transform t = mb.transform;
		UIWidget[] widgets = t.GetComponentsInChildren<UIWidget>();

		Event e = Event.current;
		int id = GUIUtility.GetControlID(mHash, FocusType.Passive);
		EventType type = e.GetTypeForControl(id);
		bool isWithinRect = false;
		Vector3[] corners = null;
		Vector3[] handles = null;

		if (widgets.Length > 0)
		{
			Matrix4x4 worldToLocal = t.worldToLocalMatrix;
			Matrix4x4 localToWorld = t.localToWorldMatrix;
			Bounds bounds = new Bounds();

			// Calculate the local bounds
			for (int i = 0; i < widgets.Length; ++i)
			{
				Vector3[] wcs = widgets[i].worldCorners;

				for (int b = 0; b < 4; ++b)
				{
					wcs[b] = worldToLocal.MultiplyPoint3x4(wcs[b]);
					if (i == 0 && b == 0) bounds = new Bounds(wcs[b], Vector3.zero);
					else bounds.Encapsulate(wcs[b]);
				}
			}

			// Calculate the 4 local corners
			Vector3 v0 = bounds.min;
			Vector3 v1 = bounds.max;

			float z = Mathf.Min(v0.z, v1.z);
			corners = new Vector3[4];
			corners[0] = new Vector3(v0.x, v0.y, z);
			corners[1] = new Vector3(v0.x, v1.y, z);
			corners[2] = new Vector3(v1.x, v1.y, z);
			corners[3] = new Vector3(v1.x, v0.y, z);

			// Transform the 4 corners into world space
			for (int i = 0; i < 4; ++i)
				corners[i] = localToWorld.MultiplyPoint3x4(corners[i]);

			handles = new Vector3[8];

			handles[0] = corners[0];
			handles[1] = corners[1];
			handles[2] = corners[2];
			handles[3] = corners[3];

			handles[4] = (corners[0] + corners[1]) * 0.5f;
			handles[5] = (corners[1] + corners[2]) * 0.5f;
			handles[6] = (corners[2] + corners[3]) * 0.5f;
			handles[7] = (corners[0] + corners[3]) * 0.5f;

			Color handlesColor = UIWidgetInspector.handlesColor;
			NGUIHandles.DrawShadowedLine(handles, handles[0], handles[1], handlesColor);
			NGUIHandles.DrawShadowedLine(handles, handles[1], handles[2], handlesColor);
			NGUIHandles.DrawShadowedLine(handles, handles[2], handles[3], handlesColor);
			NGUIHandles.DrawShadowedLine(handles, handles[0], handles[3], handlesColor);

			isWithinRect = mIsDragging || (e.modifiers == 0 &&
				NGUIEditorTools.SceneViewDistanceToRectangle(corners, e.mousePosition) == 0f);
#if !UNITY_3_5
			// Change the mouse cursor to a more appropriate one
			Vector2[] screenPos = new Vector2[8];
			for (int i = 0; i < 8; ++i) screenPos[i] = HandleUtility.WorldToGUIPoint(handles[i]);

			bounds = new Bounds(screenPos[0], Vector3.zero);
			for (int i = 1; i < 8; ++i) bounds.Encapsulate(screenPos[i]);

			// Change the cursor to a move arrow when it's within the screen rectangle
			Vector2 min = bounds.min;
			Vector2 max = bounds.max;
			Rect rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
			UIWidgetInspector.SetCursorRect(rect, isWithinRect ? MouseCursor.MoveArrow : MouseCursor.Arrow);
#endif
		}

		switch (type)
		{
			case EventType.Repaint:
			{
				if (handles != null)
				{
					Vector3 v0 = HandleUtility.WorldToGUIPoint(handles[0]);
					Vector3 v2 = HandleUtility.WorldToGUIPoint(handles[2]);

					if ((v2 - v0).magnitude > 60f)
					{
						Vector3 v1 = HandleUtility.WorldToGUIPoint(handles[1]);
						Vector3 v3 = HandleUtility.WorldToGUIPoint(handles[3]);

						Handles.BeginGUI();
						{
							for (int i = 0; i < 4; ++i)
								UIWidgetInspector.DrawKnob(handles[i], false, false, id);

							if (Mathf.Abs(v1.y - v0.y) > 80f)
							{
								UIWidgetInspector.DrawKnob(handles[4], false, false, id);
								UIWidgetInspector.DrawKnob(handles[6], false, false, id);
							}

							if (Mathf.Abs(v3.x - v0.x) > 80f)
							{
								UIWidgetInspector.DrawKnob(handles[5], false, false, id);
								UIWidgetInspector.DrawKnob(handles[7], false, false, id);
							}
						}
						Handles.EndGUI();
					}
				}
			}
			break;

			case EventType.MouseDown:
			{
				mAllowSelection = true;
				mStartMouse = e.mousePosition;

				if (e.button == 1 && isWithinRect)
				{
					GUIUtility.hotControl = GUIUtility.keyboardControl = id;
					e.Use();
				}
				else if (e.button == 0 && isWithinRect && corners != null && UIWidgetInspector.Raycast(corners, out mStartDrag))
				{
					mCanDrag = true;
					mStartPos = t.position;
					GUIUtility.hotControl = GUIUtility.keyboardControl = id;
					e.Use();
				}
			}
			break;

			case EventType.MouseDrag:
			{
				// Prevent selection once the drag operation begins
				bool dragStarted = (e.mousePosition - mStartMouse).magnitude > 3f;
				if (dragStarted) mAllowSelection = false;

				if (GUIUtility.hotControl == id)
				{
					e.Use();

					if (mCanDrag)
					{
						Vector3 pos;

						if (corners != null & UIWidgetInspector.Raycast(corners, out pos))
						{
							// Wait until the mouse moves by more than a few pixels
							if (!mIsDragging && dragStarted)
							{
								NGUIEditorTools.RegisterUndo("Move " + t.name, t);
								mStartPos = t.position;
								mIsDragging = true;
							}

							if (mIsDragging)
							{
								t.position = mStartPos + (pos - mStartDrag);
								pos = t.localPosition;
								pos.x = Mathf.Round(pos.x);
								pos.y = Mathf.Round(pos.y);
								pos.z = Mathf.Round(pos.z);
								t.localPosition = pos;
							}
						}
					}
				}
			}
			break;

			case EventType.MouseUp:
			{
				if (GUIUtility.hotControl == id)
				{
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;

					if (e.button == 0)
					{
						if (mIsDragging)
						{
							mIsDragging = false;
							Vector3 pos = t.localPosition;
							pos.x = Mathf.Round(pos.x);
							pos.y = Mathf.Round(pos.y);
							pos.z = Mathf.Round(pos.z);
							t.localPosition = pos;
						}
						else if (mAllowSelection)
						{
							// Left-click: Select the topmost widget
							NGUIEditorTools.SelectWidget(e.mousePosition);
							e.Use();
						}
						e.Use();
					}
					else
					{
						// Right-click: Open a context menu listing all widgets underneath
						NGUIEditorTools.ShowSpriteSelectionMenu(e.mousePosition);
						e.Use();
					}
					mCanDrag = false;
				}
			}
			break;

			case EventType.KeyDown:
			{
				if (e.keyCode == KeyCode.UpArrow)
				{
					Vector3 pos = t.localPosition;
					pos.y += 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.DownArrow)
				{
					Vector3 pos = t.localPosition;
					pos.y -= 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.LeftArrow)
				{
					Vector3 pos = t.localPosition;
					pos.x -= 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.RightArrow)
				{
					Vector3 pos = t.localPosition;
					pos.x += 1f;
					t.localPosition = pos;
					e.Use();
				}
				else if (e.keyCode == KeyCode.Escape)
				{
					if (GUIUtility.hotControl == id)
					{
						if (mIsDragging)
						{
							mIsDragging = false;
							t.position = mStartPos;
						}

						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
						e.Use();
					}
					else
					{
						Selection.activeGameObject = null;
					}
				}
			}
			break;
		}
	}
}
