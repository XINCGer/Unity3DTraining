//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

/// <summary>
/// Tool that makes it easy to drag prefabs into it to "cache" them for ease of use.
/// </summary>

public class UIPrefabTool : EditorWindow
{
	static public UIPrefabTool instance;

	class Item
	{
		public GameObject prefab;
		public string guid;
		public Texture tex;
		public bool dynamicTex = false;
	}

	enum Mode
	{
		CompactMode,
		IconMode,
		DetailedMode,
	}

	const int cellPadding = 4;

	int cellSize { get { return (mMode == Mode.CompactMode) ? 50 : 80; } }

	int mTab = 0;
	Mode mMode = Mode.IconMode;
	Vector2 mPos = Vector2.zero;
	bool mMouseIsInside = false;
	GUIContent mContent;
	GUIStyle mStyle;

	// List of all the added objects
	BetterList<Item> mItems = new BetterList<Item>();

	/// <summary>
	/// Get or set the dragged object.
	/// </summary>

	GameObject draggedObject
	{
		get
		{
			if (DragAndDrop.objectReferences == null) return null;
			if (DragAndDrop.objectReferences.Length == 1) return DragAndDrop.objectReferences[0] as GameObject;
			return null;
		}
		set
		{
			if (value != null)
			{
				DragAndDrop.PrepareStartDrag();
				DragAndDrop.objectReferences = new Object[1] { value };
				draggedObjectIsOurs = true;
			}
			else DragAndDrop.AcceptDrag();
		}
	}

	/// <summary>
	/// Whether the dragged object is coming from the outside (new object) or from the window (cloned object).
	/// </summary>

	bool draggedObjectIsOurs
	{
		get
		{
			object obj = DragAndDrop.GetGenericData("Prefab Tool");
			if (obj == null) return false;
			return (bool)obj;
		}
		set
		{
			DragAndDrop.SetGenericData("Prefab Tool", value);
		}
	}

	/// <summary>
	/// Initialize everything.
	/// </summary>

	void OnEnable ()
	{
		instance = this;

		Load();

		mContent = new GUIContent();
		mStyle = new GUIStyle();
		mStyle.alignment = TextAnchor.MiddleCenter;
		mStyle.padding = new RectOffset(2, 2, 2, 2);
		mStyle.clipping = TextClipping.Clip;
		mStyle.wordWrap = true;
		mStyle.stretchWidth = false;
		mStyle.stretchHeight = false;
		mStyle.normal.textColor = UnityEditor.EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f);
		mStyle.normal.background = null;
	}

	/// <summary>
	/// Clean up all textures.
	/// </summary>

	void OnDisable ()
	{
		instance = null;
		foreach (Item item in mItems) DestroyTexture(item);
		Save();
	}

	void OnSelectionChange () { Repaint(); }

	/// <summary>
	/// Reset all loaded prefabs, collecting default controls instead.
	/// </summary>

	public void Reset ()
	{
		foreach (Item item in mItems) DestroyTexture(item);
		mItems.Clear();

		if (mTab == 0)
		{
			List<string> filtered = new List<string>();
			string[] allAssets = AssetDatabase.GetAllAssetPaths();

			foreach (string s in allAssets)
			{
				if (s.EndsWith(".prefab") && s.Contains("Control -"))
					filtered.Add(s);
			}

			filtered.Sort(string.Compare);
			foreach (string s in filtered) AddGUID(AssetDatabase.AssetPathToGUID(s), -1);
			RectivateLights();
		}
	}

	/// <summary>
	/// Add a new item to the list.
	/// </summary>

	void AddItem (GameObject go, int index)
	{
		string guid = NGUIEditorTools.ObjectToGUID(go);

		if (string.IsNullOrEmpty(guid))
		{
#if UNITY_3_5
			string path = EditorUtility.SaveFilePanel("Save a prefab",
				NGUISettings.currentPath, go.name + ".prefab", "prefab");
#else
			string path = EditorUtility.SaveFilePanelInProject("Save a prefab",
				go.name + ".prefab", "prefab", "Save prefab as...", NGUISettings.currentPath);
#endif	
			if (string.IsNullOrEmpty(path)) return;
			NGUISettings.currentPath = System.IO.Path.GetDirectoryName(path);

			go = PrefabUtility.CreatePrefab(path, go);
			if (go == null) return;

			guid = NGUIEditorTools.ObjectToGUID(go);
			if (string.IsNullOrEmpty(guid)) return;
		}

		Item ent = new Item();
		ent.prefab = go;
		ent.guid = guid;
		GeneratePreview(ent, null);
		RectivateLights();

		if (index < mItems.size) mItems.Insert(index, ent);
		else mItems.Add(ent);
		Save();
	}

	/// <summary>
	/// Add a new item to the list.
	/// </summary>

	Item AddGUID (string guid, int index)
	{
		GameObject go = NGUIEditorTools.GUIDToObject<GameObject>(guid);

		if (go != null)
		{
			Item ent = new Item();
			ent.prefab = go;
			ent.guid = guid;
			GeneratePreview(ent, null);
			if (index < mItems.size) mItems.Insert(index, ent);
			else mItems.Add(ent);
			return ent;
		}
		return null;
	}

	/// <summary>
	/// Remove an existing item from the list.
	/// </summary>

	void RemoveItem (object obj)
	{
		if (this == null) return;
		int index = (int)obj;
		if (index < mItems.size && index > -1)
		{
			Item item = mItems[index];
			DestroyTexture(item);
			mItems.RemoveAt(index);
		}
		Save();
	}

	/// <summary>
	/// Find an item referencing the specified game object.
	/// </summary>

	Item FindItem (GameObject go)
	{
		for (int i = 0; i < mItems.size; ++i)
			if (mItems[i].prefab == go)
				return mItems[i];
		return null;
	}

	/// <summary>
	/// Key used to save and load the data.
	/// </summary>

	string saveKey { get { return "NGUI " + Application.dataPath + " " + mTab; } }

	/// <summary>
	/// Save all the items to Editor Prefs.
	/// </summary>

	void Save ()
	{
		string data = "";

		if (mItems.size > 0)
		{
			string guid = mItems[0].guid;
			StringBuilder sb = new StringBuilder();
			sb.Append(guid);

			for (int i = 1; i < mItems.size; ++i)
			{
				guid = mItems[i].guid;

				if (string.IsNullOrEmpty(guid))
				{
					Debug.LogWarning("Unable to save " + mItems[i].prefab.name);
				}
				else
				{
					sb.Append('|');
					sb.Append(mItems[i].guid);
				}
			}
			data = sb.ToString();
		}
		NGUISettings.SetString(saveKey, data);
	}

	/// <summary>
	/// Load all items from Editor Prefs.
	/// </summary>

	void Load ()
	{
		mTab = NGUISettings.GetInt("NGUI Prefab Tab", 0);
		mMode = NGUISettings.GetEnum<Mode>("NGUI Prefab Mode", mMode);

		foreach (Item item in mItems) DestroyTexture(item);
		mItems.Clear();

		string data = NGUISettings.GetString(saveKey, "");

		if (string.IsNullOrEmpty(data))
		{
			Reset();
		}
		else
		{
			if (string.IsNullOrEmpty(data)) return;
			string[] guids = data.Split('|');
			foreach (string s in guids) AddGUID(s, -1);
			RectivateLights();
		}
	}

	/// <summary>
	/// Destroy the item's texture.
	/// </summary>

	void DestroyTexture (Item item)
	{
		if (item != null && item.dynamicTex && item.tex != null)
		{
			DestroyImmediate(item.tex);
			item.dynamicTex = false;
			item.tex = null;
		}
	}

	/// <summary>
	/// Re-generate the specified prefab's snapshot texture using the provided snapshot point's values.
	/// </summary>

	public void RegenerateTexture (GameObject prefab, UISnapshotPoint point)
	{
		for (int i = 0; i < mItems.size; ++i)
		{
			Item item = mItems[i];

			if (item.prefab == prefab)
			{
				GeneratePreview(item, point);
				RectivateLights();
				break;
			}
		}
	}

	/// <summary>
	/// Update the visual mode based on the dragged object.
	/// </summary>

	void UpdateVisual ()
	{
		if (draggedObject == null) DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
		else if (draggedObjectIsOurs) DragAndDrop.visualMode = DragAndDropVisualMode.Move;
		else DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
	}

	/// <summary>
	/// Helper function that creates a new entry using the specified object's path.
	/// </summary>

	Item CreateItemByPath (string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			path = FileUtil.GetProjectRelativePath(path);
			string guid = AssetDatabase.AssetPathToGUID(path);

			if (!string.IsNullOrEmpty(guid))
			{
				GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
				Item ent = new Item();
				ent.prefab = go;
				ent.guid = guid;
				GeneratePreview(ent, null);
				return ent;
			}
			else Debug.Log("No GUID");
		}
		return null;
	}

	/// <summary>
	/// GetComponentInChildren doesn't work on prefabs.
	/// </summary>

	static UISnapshotPoint GetSnapshotPoint (Transform t)
	{
		UISnapshotPoint point = t.GetComponent<UISnapshotPoint>();
		if (point != null) return point;
		
		for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform c = t.GetChild(i);
			point = GetSnapshotPoint(c);
			if (point != null) return point;
		}
		return null;
	}

	/// <summary>
	/// Generate an item preview for the specified item.
	/// </summary>

	void GeneratePreview (Item item, UISnapshotPoint point)
	{
		if (item == null || item.prefab == null) return;

		// For some reason Unity 5 doesn't seem to support render textures at edit time while Unity 4 does...
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		if (point == null) point = GetSnapshotPoint(item.prefab.transform);

		if (point != null && point.thumbnail != null)
		{
			// Explicitly chosen thumbnail
			item.tex = point.thumbnail;
			item.dynamicTex = false;
			return;
		}
		else if (!UnityEditorInternal.InternalEditorUtility.HasPro())
#endif
		{
			// Render textures only work in Unity Pro
			string path = "Assets/NGUI/Editor/Preview/" + item.prefab.name + ".png";
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			item.tex = File.Exists(path) ? (Texture2D)Resources.LoadAssetAtPath(path, typeof(Texture2D)) : null;
#else
			item.tex = File.Exists(path) ? (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) : null;
#endif
			item.dynamicTex = false;
			return;
		}
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		int dim = (cellSize - 4) * 2;

		// Asset Preview-based approach is unreliable, and most of the time fails to provide a texture.
		// Sometimes it even throws null exceptions.
		//item.tex = AssetPreview.GetAssetPreview(item.prefab);
		//item.dynamicTex = false;
		//if (item.tex != null) return;

		// Let's create a basic scene
		GameObject root = EditorUtility.CreateGameObjectWithHideFlags("Preview Root", HideFlags.HideAndDontSave);
		GameObject camGO = EditorUtility.CreateGameObjectWithHideFlags("Preview Camera", HideFlags.HideAndDontSave, typeof(Camera));

		// Position it far away so that it doesn't interfere with existing objects
		root.transform.position = new Vector3(0f, 0f, 10000f);
		root.layer = item.prefab.layer;

		// Set up the camera
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		Camera cam = camGO.camera;
		cam.isOrthoGraphic = true;
#else
		Camera cam = camGO.GetComponent<Camera>();
		cam.orthographic = true;
#endif
		cam.renderingPath = RenderingPath.Forward;
		cam.clearFlags = CameraClearFlags.Skybox;
		cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		cam.targetTexture = (item.tex as RenderTexture);
		cam.enabled = false;

		// Finally instantiate the prefab as a child of the root
		GameObject child = NGUITools.AddChild(root, item.prefab);

		// Try to find the snapshot point script
		if (point == null) point = child.GetComponentInChildren<UISnapshotPoint>();

		// If there is a UIRect present (widgets or panels) then it's an NGUI object
		RenderTexture rt = (SetupPreviewForUI(cam, root, child, point) || SetupPreviewFor3D(cam, root, child, point)) ?
			cam.RenderToTexture(dim, dim) : null;

		// Did we have a different render texture? Get rid of it.
		if (item.tex != rt && item.tex != null && item.dynamicTex)
		{
			NGUITools.DestroyImmediate(item.tex);
			item.tex = null;
			item.dynamicTex = false;
		}

		// Do we have a new render texture? Assign it.
		if (rt != null)
		{
			item.tex = rt;
			item.dynamicTex = true;
		}

		// Clean up everything
		DestroyImmediate(camGO);
		DestroyImmediate(root);
#endif
	}

	/// <summary>
	/// Set up everything necessary to preview a UI object.
	/// </summary>

	static bool SetupPreviewForUI (Camera cam, GameObject root, GameObject child, UISnapshotPoint point)
	{
		if (child.GetComponentInChildren<UIRect>() == null) return false;

		if (child.GetComponent<UIPanel>() == null)
			root.AddComponent<UIPanel>();

		Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(child.transform);
		Vector3 size = bounds.extents;
		float objSize = size.magnitude;

		cam.transform.position = bounds.center;
		cam.cullingMask = (1 << root.layer);

		if (point != null) SetupSnapshotCamera(child, cam, point);
		else SetupSnapshotCamera(child, cam, objSize, Mathf.RoundToInt(Mathf.Max(size.x, size.y)), -100f, 100f);
		NGUITools.ImmediatelyCreateDrawCalls(root);
		return true;
	}

	/// <summary>
	/// Set up everything necessary to preview a UI object.
	/// </summary>

	static bool SetupPreviewFor3D (Camera cam, GameObject root, GameObject child, UISnapshotPoint point)
	{
		Renderer[] rens = child.GetComponentsInChildren<Renderer>();
		if (rens.Length == 0) return false;

		Vector3 camDir = new Vector3(-0.25f, -0.35f, -0.5f);
		Vector3 lightDir = new Vector3(-0.25f, -0.5f, -0.25f);

		camDir.Normalize();
		lightDir.Normalize();

		// Determine the bounds of the model
		Renderer ren = rens[0];
		Bounds bounds = ren.bounds;
		int mask = (1 << ren.gameObject.layer);

		for (int i = 1; i < rens.Length; ++i)
		{
			ren = rens[i];
			mask |= (1 << ren.gameObject.layer);
			bounds.Encapsulate(ren.bounds);
		}

		// Set the camera's properties
		cam.cullingMask = mask;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		cam.isOrthoGraphic = true;
#else
		cam.orthographic = true;
#endif
		cam.transform.position = bounds.center;
		cam.transform.rotation = Quaternion.LookRotation(camDir);

		float objSize = bounds.size.magnitude;
		if (point != null) SetupSnapshotCamera(child, cam, point);
		else SetupSnapshotCamera(child, cam, objSize, objSize * 0.4f, -objSize, objSize);

		// Deactivate all scene lights
		DeactivateLights();

		// Create our own light
		GameObject lightGO = NGUITools.AddChild(root);
		Light light = lightGO.AddComponent<Light>();
		light.type = LightType.Directional;
		light.shadows = LightShadows.None;
		light.color = Color.white;
		light.intensity = 0.65f;
		light.transform.rotation = Quaternion.LookRotation(lightDir);
		light.cullingMask = mask;
		return true;
	}

	/// <summary>
	/// Set up the camera using the provided snapshot point's values.
	/// </summary>

	static void SetupSnapshotCamera (GameObject go, Camera cam, UISnapshotPoint point)
	{
		Vector3 pos = point.transform.localPosition;
		Quaternion rot = point.transform.localRotation;
		Transform t = go.transform;

		if (t.parent != null)
		{
			pos = t.parent.TransformPoint(pos);
			rot = t.parent.rotation * rot;
		}

		cam.transform.position = pos;
		cam.transform.rotation = rot;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		cam.isOrthoGraphic = point.isOrthographic;
#else
		cam.orthographic = point.isOrthographic;
#endif
		cam.nearClipPlane = point.nearClip;
		cam.farClipPlane = point.farClip;
		cam.orthographicSize = point.orthoSize;
		cam.fieldOfView = point.fieldOfView;
	}

	/// <summary>
	/// Set up the snapshot camera using an explicit game object, if there is one available.
	/// </summary>

	static void SetupSnapshotCamera (GameObject go, Camera cam, float objectSize, float orthoSize, float near, float far)
	{
		// If you place a game object called "NGUI Snapshot Point" on your object,
		// NGUI will use it as the camera's snapshot point, taking its position, rotation,
		// and any optional parameters you deem to specify. For an orthographic snapshot,
		// specify only one parameter -- the orthographic camera's size.
		// For a 3D snapshot, specify 3 parameters: near, far, and field of view.
		// Parameters must be separated by a space. For example:
		//   NGUI Snapshot Point 0.3
		//   NGUI Snapshot Point 0.1 10 45

		Transform snapshot = FindChild(go.transform, "NGUI Snapshot Point");
		
		if (snapshot == null)
		{
			cam.nearClipPlane = near;
			cam.farClipPlane = far;
			cam.orthographicSize = orthoSize;
			return;
		}

		string str = snapshot.name.Replace("NGUI Snapshot Point", "");
		string[] parts = str.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length == 3)
		{
			near = 0.1f;
			far = objectSize * 3f;
			float fov = 30f;

			float.TryParse(parts[0], out near);
			float.TryParse(parts[1], out far);
			float.TryParse(parts[2], out fov);

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
			cam.isOrthoGraphic = false;
#else
			cam.orthographic = false;
#endif
			cam.nearClipPlane = near;
			cam.farClipPlane = far;
			cam.fieldOfView = fov;
		}
		else if (parts.Length > 0)
		{
			float.TryParse(parts[0], out orthoSize);
			cam.nearClipPlane = near;
			cam.farClipPlane = far;
			cam.orthographicSize = orthoSize;
		}

		cam.transform.position = snapshot.position;
		cam.transform.rotation = snapshot.rotation;
	}

	/// <summary>
	/// Find a child with a name that begins with the specified string.
	/// </summary>

	static Transform FindChild (Transform t, string startsWith)
	{
		if (t.name.StartsWith(startsWith)) return t;

		for (int i = 0, imax = t.childCount; i < imax; ++i)
		{
			Transform ch = FindChild(t.GetChild(i), startsWith);
			if (ch != null) return ch;
		}
		return null;
	}

	// List of lights that have been deactivated
	static BetterList<Light> mLights;

	/// <summary>
	/// Deactivate all scene lights.
	/// </summary>

	static void DeactivateLights ()
	{
		if (mLights == null)
		{
			mLights = new BetterList<Light>();
			Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];

			foreach (Light l in lights)
			{
				if (NGUITools.GetActive(l))
				{
					l.enabled = false;
					mLights.Add(l);
				}
			}
		}
	}

	/// <summary>
	/// Reactivate all scene lights.
	/// </summary>

	static void RectivateLights ()
	{
		if (mLights != null)
		{
			for (int i = 0; i < mLights.size; ++i)
				mLights[i].enabled = true;
			mLights = null;
		}
	}

	/// <summary>
	/// Helper function that retrieves the index of the cell under the mouse.
	/// </summary>

	int GetCellUnderMouse (int spacingX, int spacingY)
	{
		Vector2 pos = Event.current.mousePosition + mPos;

		int topPadding = 40; // Account for mode and search bars
		int x = cellPadding, y = cellPadding + topPadding;
		if (pos.y < y) return -1;

		float width = Screen.width - cellPadding + mPos.x;
		float height = Screen.height - cellPadding + mPos.y;
		int index = 0;

		for (; ; ++index)
		{
			Rect rect = new Rect(x, y, spacingX, spacingY);
			if (rect.Contains(pos)) break;

			x += spacingX;

			if (x + spacingX > width)
			{
				if (pos.x > x) return -1;
				y += spacingY;
				x = cellPadding;
				if (y + spacingY > height) return -1;
			}
		}
		return index;
	}

	bool mReset = false;

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		Event currentEvent = Event.current;
		EventType type = currentEvent.type;

		int x = cellPadding, y = cellPadding;
		int width = Screen.width - cellPadding;
		int spacingX = cellSize + cellPadding;
		int spacingY = spacingX;
		if (mMode == Mode.DetailedMode) spacingY += 32;

		GameObject dragged = draggedObject;
		bool isDragging = (dragged != null);
		int indexUnderMouse = GetCellUnderMouse(spacingX, spacingY);
		Item selection = isDragging ? FindItem(dragged) : null;
		string searchFilter = NGUISettings.searchField;

		int newTab = mTab;

		GUILayout.BeginHorizontal();
		if (GUILayout.Toggle(newTab == 0, "1", "ButtonLeft")) newTab = 0;
		if (GUILayout.Toggle(newTab == 1, "2", "ButtonMid")) newTab = 1;
		if (GUILayout.Toggle(newTab == 2, "3", "ButtonMid")) newTab = 2;
		if (GUILayout.Toggle(newTab == 3, "4", "ButtonMid")) newTab = 3;
		if (GUILayout.Toggle(newTab == 4, "5", "ButtonRight")) newTab = 4;
		GUILayout.EndHorizontal();

		if (mTab != newTab)
		{
			Save();
			mTab = newTab;
			mReset = true;
			NGUISettings.SetInt("NGUI Prefab Tab", mTab);
			Load();
		}

		if (mReset && type == EventType.Repaint)
		{
			mReset = false;
			foreach (Item item in mItems) GeneratePreview(item, null);
			RectivateLights();
		}

		// Search field
		GUILayout.BeginHorizontal();
		{
			string after = EditorGUILayout.TextField("", searchFilter, "SearchTextField", GUILayout.Width(Screen.width - 20f));

			if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
			{
				after = "";
				GUIUtility.keyboardControl = 0;
			}

			if (searchFilter != after)
			{
				NGUISettings.searchField = after;
				searchFilter = after;
			}
		}
		GUILayout.EndHorizontal();

		bool eligibleToDrag = (currentEvent.mousePosition.y < Screen.height - 40);

		if (type == EventType.MouseDown)
		{
			mMouseIsInside = true;
		}
		else if (type == EventType.MouseDrag)
		{
			mMouseIsInside = true;

			if (indexUnderMouse != -1 && eligibleToDrag)
			{
				// Drag operation begins
				if (draggedObjectIsOurs) DragAndDrop.StartDrag("Prefab Tool");
				currentEvent.Use();
			}
		}
		else if (type == EventType.MouseUp)
		{
			DragAndDrop.PrepareStartDrag();
			mMouseIsInside = false;
			Repaint();
		}
		else if (type == EventType.DragUpdated)
		{
			// Something dragged into the window
			mMouseIsInside = true;
			UpdateVisual();
			currentEvent.Use();
		}
		else if (type == EventType.DragPerform)
		{
			// We've dropped a new object into the window
			if (dragged != null)
			{
				if (selection != null)
				{
					DestroyTexture(selection);
					mItems.Remove(selection);
				}

				AddItem(dragged, indexUnderMouse);
				draggedObject = null;
			}
			mMouseIsInside = false;
			currentEvent.Use();
		}
		else if (type == EventType.DragExited || type == EventType.Ignore)
		{
			mMouseIsInside = false;
		}

		// If the mouse is not inside the window, clear the selection and dragged object
		if (!mMouseIsInside)
		{
			selection = null;
			dragged = null;
		}

		// Create a list of indices, inserting an entry of '-1' underneath the dragged object
		BetterList<int> indices = new BetterList<int>();

		for (int i = 0; i < mItems.size; )
		{
			if (dragged != null && indices.size == indexUnderMouse)
				indices.Add(-1);

			if (mItems[i] != selection)
			{
				if (string.IsNullOrEmpty(searchFilter) ||
					mItems[i].prefab.name.IndexOf(searchFilter, System.StringComparison.CurrentCultureIgnoreCase) != -1)
						indices.Add(i);
			}
			++i;
		}

		// There must always be '-1' (Add/Move slot) present
		if (!indices.Contains(-1)) indices.Add(-1);

		// We want to start dragging something from within the window
		if (eligibleToDrag && type == EventType.MouseDown && indexUnderMouse > -1)
		{
			GUIUtility.keyboardControl = 0;

			if (currentEvent.button == 0 && indexUnderMouse < indices.size)
			{
				int index = indices[indexUnderMouse];

				if (index != -1 && index < mItems.size)
				{
					selection = mItems[index];
					draggedObject = selection.prefab;
					dragged = selection.prefab;
					currentEvent.Use();
				}
			}
		}
		//else if (type == EventType.MouseUp && currentEvent.button == 1 && indexUnderMouse > mItems.size)
		//{
		//    NGUIContextMenu.AddItem("Reset", false, RemoveItem, index);
		//    NGUIContextMenu.Show();
		//}

		// Draw the scroll view with prefabs
		mPos = GUILayout.BeginScrollView(mPos);
		{
			Color normal = new Color(1f, 1f, 1f, 0.5f);

			for (int i = 0; i < indices.size; ++i)
			{
				int index = indices[i];
				Item ent = (index != -1) ? mItems[index] : selection;

				if (ent != null && ent.prefab == null)
				{
					mItems.RemoveAt(index);
					continue;
				}

				Rect rect = new Rect(x, y, cellSize, cellSize);
				Rect inner = rect;
				inner.xMin += 2f;
				inner.xMax -= 2f;
				inner.yMin += 2f;
				inner.yMax -= 2f;
				rect.yMax -= 1f; // Button seems to be mis-shaped. It's height is larger than its width by a single pixel.

				if (!isDragging && (mMode == Mode.CompactMode || (ent == null || ent.tex != null)))
					mContent.tooltip = (ent != null) ? ent.prefab.name : "Click to add";
				else mContent.tooltip = "";

				//if (ent == selection)
				{
					GUI.color = normal;
					NGUIEditorTools.DrawTiledTexture(inner, NGUIEditorTools.backdropTexture);
				}

				GUI.color = Color.white;
				GUI.backgroundColor = normal;

				if (GUI.Button(rect, mContent, "Button"))
				{
					if (ent == null || currentEvent.button == 0)
					{
						string path = EditorUtility.OpenFilePanel("Add a prefab", NGUISettings.currentPath, "prefab");

						if (!string.IsNullOrEmpty(path))
						{
							NGUISettings.currentPath = System.IO.Path.GetDirectoryName(path);
							Item newEnt = CreateItemByPath(path);

							if (newEnt != null)
							{
								mItems.Add(newEnt);
								Save();
							}
						}
					}
					else if (currentEvent.button == 1)
					{
						NGUIContextMenu.AddItem("Delete", false, RemoveItem, index);
						NGUIContextMenu.Show();
					}
				}

				string caption = (ent == null) ? "" : ent.prefab.name.Replace("Control - ", "");

				if (ent != null)
				{
					if (ent.tex != null)
					{
						GUI.DrawTexture(inner, ent.tex);
					}
					else if (mMode != Mode.DetailedMode)
					{
						GUI.Label(inner, caption, mStyle);
						caption = "";
					}
				}
				else GUI.Label(inner, "Add", mStyle);

				if (mMode == Mode.DetailedMode)
				{
					GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
					GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
					GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), caption, "ProgressBarBack");
					GUI.contentColor = Color.white;
					GUI.backgroundColor = Color.white;
				}

				x += spacingX;

				if (x + spacingX > width)
				{
					y += spacingY;
					x = cellPadding;
				}
			}
			GUILayout.Space(y);
		}
		GUILayout.EndScrollView();

		// Mode
		Mode modeAfter = (Mode)EditorGUILayout.EnumPopup(mMode);

		if (modeAfter != mMode)
		{
			mMode = modeAfter;
			mReset = true;
			NGUISettings.SetEnum("NGUI Prefab Mode", mMode);
		}
	}
}
