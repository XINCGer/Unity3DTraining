//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Panel wizard that allows a bird's eye view of all cameras in your scene.
/// </summary>

public class UICameraTool : EditorWindow
{
	Vector2 mScroll = Vector2.zero;

	/// <summary>
	/// Layer mask field, originally from:
	/// http://answers.unity3d.com/questions/60959/mask-field-in-the-editor.html
	/// </summary>

	static public int LayerMaskField (string label, int mask, params GUILayoutOption[] options)
	{
		List<string> layers = new List<string>();
		List<int> layerNumbers = new List<int>();

		string selectedLayers = "";

		for (int i = 0; i < 32; ++i)
		{
			string layerName = LayerMask.LayerToName(i);

			if (!string.IsNullOrEmpty(layerName))
			{
				if (mask == (mask | (1 << i)))
				{
					if (string.IsNullOrEmpty(selectedLayers))
					{
						selectedLayers = layerName;
					}
					else
					{
						selectedLayers = "Mixed";
					}
				}
			}
		}

		if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
		{
			if (mask == 0)
			{
				layers.Add("Nothing");
			}
			else if (mask == -1)
			{
				layers.Add("Everything");
			}
			else
			{
				layers.Add(selectedLayers);
			}
			layerNumbers.Add(-1);
		}

		layers.Add((mask == 0 ? "[+] " : "      ") + "Nothing");
		layerNumbers.Add(-2);

		layers.Add((mask == -1 ? "[+] " : "      ") + "Everything");
		layerNumbers.Add(-3);

		for (int i = 0; i < 32; ++i)
		{
			string layerName = LayerMask.LayerToName(i);

			if (layerName != "")
			{
				if (mask == (mask | (1 << i)))
				{
					layers.Add("[+] " + layerName);
				}
				else
				{
					layers.Add("      " + layerName);
				}
				layerNumbers.Add(i);
			}
		}

		bool preChange = GUI.changed;

		GUI.changed = false;

		int newSelected = 0;

		if (Event.current.type == EventType.MouseDown)
		{
			newSelected = -1;
		}

		if (string.IsNullOrEmpty(label))
		{
			newSelected = EditorGUILayout.Popup(newSelected, layers.ToArray(), EditorStyles.layerMaskField, options);
		}
		else
		{
			newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField, options);
		}

		if (GUI.changed && newSelected >= 0)
		{
			if (newSelected == 0)
			{
				mask = 0;
			}
			else if (newSelected == 1)
			{
				mask = -1;
			}
			else
			{
				if (mask == (mask | (1 << layerNumbers[newSelected])))
				{
					mask &= ~(1 << layerNumbers[newSelected]);
				}
				else
				{
					mask = mask | (1 << layerNumbers[newSelected]);
				}
			}
		}
		else
		{
			GUI.changed = preChange;
		}
		return mask;
	}

	static public int LayerMaskField (int mask, params GUILayoutOption[] options)
	{
		return LayerMaskField(null, mask, options);
	}

	/// <summary>
	/// Refresh the window on selection.
	/// </summary>

	void OnSelectionChange () { Repaint(); }

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);

		List<Camera> list = NGUIEditorTools.FindAll<Camera>();

		if (list.Count > 0)
		{
			DrawRow(null);
			NGUIEditorTools.DrawSeparator();
			mScroll = GUILayout.BeginScrollView(mScroll);
			foreach (Camera cam in list) DrawRow(cam);
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label("No cameras found in the scene");
		}
	}

	/// <summary>
	/// Helper function used to print things in columns.
	/// </summary>

	void DrawRow (Camera cam)
	{
		bool highlight = (cam == null || Selection.activeGameObject == null) ? false :
				(0 != (cam.cullingMask & (1 << Selection.activeGameObject.layer)));

		if (cam != null)
		{
			GUI.backgroundColor = highlight ? Color.white : new Color(0.8f, 0.8f, 0.8f);
			GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
			GUI.backgroundColor = Color.white;
		}
		else
		{
			GUILayout.BeginHorizontal();
		}

		bool enabled = (cam == null || (NGUITools.GetActive(cam.gameObject) && cam.enabled));

		GUI.color = Color.white;

		if (cam != null)
		{
			if (enabled != EditorGUILayout.Toggle(enabled, GUILayout.Width(20f)))
			{
				cam.enabled = !enabled;
				EditorUtility.SetDirty(cam.gameObject);
			}
		}
		else
		{
			GUILayout.Space(30f);
		}

		if (enabled)
		{
			GUI.color = highlight ? new Color(0f, 0.8f, 1f) : Color.white;
		}
		else
		{
			GUI.color = highlight ? new Color(0f, 0.5f, 0.8f) : Color.grey;
		}

		string camName, camLayer;

		if (cam == null)
		{
			camName = "Camera's Name";
			camLayer = "Layer";
		}
		else
		{
			camName = cam.name + (cam.orthographic ? " (2D)" : " (3D)");
			camLayer = LayerMask.LayerToName(cam.gameObject.layer);
		}

		if (GUILayout.Button(camName, EditorStyles.label, GUILayout.MinWidth(100f)) && cam != null)
		{
			Selection.activeGameObject = cam.gameObject;
			EditorUtility.SetDirty(cam.gameObject);
		}
		GUILayout.Label(camLayer, GUILayout.Width(70f));

		GUI.color = enabled ? Color.white : new Color(0.7f, 0.7f, 0.7f);

		if (cam == null)
		{
			GUILayout.Label("EV", GUILayout.Width(26f));
		}
		else
		{
			UICamera uic = cam.GetComponent<UICamera>();
			bool ev = (uic != null && uic.enabled);

			if (ev != EditorGUILayout.Toggle(ev, GUILayout.Width(20f)))
			{
				if (uic == null) uic = cam.gameObject.AddComponent<UICamera>();
				uic.enabled = !ev;
			}
		}

		if (cam == null)
		{
			GUILayout.Label("Mask", GUILayout.Width(100f));
		}
		else
		{
			int mask = LayerMaskField(cam.cullingMask, GUILayout.Width(105f));

			if (cam.cullingMask != mask)
			{
				NGUIEditorTools.RegisterUndo("Camera Mask Change", cam);
				cam.cullingMask = mask;
			}
		}
		GUILayout.EndHorizontal();
	}
}
