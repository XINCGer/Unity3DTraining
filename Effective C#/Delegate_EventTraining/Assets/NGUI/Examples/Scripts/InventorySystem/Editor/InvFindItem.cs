using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory System search functionality.
/// </summary>

public class InvFindItem : ScriptableWizard
{
	/// <summary>
	/// Private class used to return data from the Find function below.
	/// </summary>

	struct FindResult
	{
		public InvDatabase db;
		public InvBaseItem item;
	}

	string mItemName = "";
	List<FindResult> mResults = new List<FindResult>();

	/// <summary>
	/// Add a menu option to display this wizard.
	/// </summary>

	[MenuItem("Window/Find Item #&i")]
	static void FindItem ()
	{
		ScriptableWizard.DisplayWizard<InvFindItem>("Find Item");
	}

	/// <summary>
	/// Draw the custom wizard.
	/// </summary>

	void OnGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);
		string newItemName = EditorGUILayout.TextField("Search for:", mItemName);
		NGUIEditorTools.DrawSeparator();

		if (GUI.changed || newItemName != mItemName)
		{
			mItemName = newItemName;

			if (string.IsNullOrEmpty(mItemName))
			{
				mResults.Clear();
			}
			else
			{
				FindAllByName(mItemName);
			}
		}

		if (mResults.Count == 0)
		{
			if (!string.IsNullOrEmpty(mItemName))
			{
				GUILayout.Label("No matches found");
			}
		}
		else
		{
			Print3("Item ID", "Item Name", "Path", false);
			NGUIEditorTools.DrawSeparator();

			foreach (FindResult fr in mResults)
			{
				if (Print3(InvDatabase.FindItemID(fr.item).ToString(),
					fr.item.name, NGUITools.GetHierarchy(fr.db.gameObject), true))
				{
					InvDatabaseInspector.SelectIndex(fr.db, fr.item);
					Selection.activeGameObject = fr.db.gameObject;
					EditorUtility.SetDirty(Selection.activeGameObject);
				}
			}
		}
	}

	/// <summary>
	/// Helper function used to print things in columns.
	/// </summary>

	bool Print3 (string a, string b, string c, bool button)
	{
		bool retVal = false;

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label(a, GUILayout.Width(80f));
			GUILayout.Label(b, GUILayout.Width(160f));
			GUILayout.Label(c);

			if (button)
			{
				retVal = GUILayout.Button("Select", GUILayout.Width(60f));
			}
			else
			{
				GUILayout.Space(60f);
			}
		}
		GUILayout.EndHorizontal();
		return retVal;
	}

	/// <summary>
	/// Find items by name.
	/// </summary>

	void FindAllByName (string partial)
	{
		partial = partial.ToLower();
		mResults.Clear();

		// Exact match comes first
		foreach (InvDatabase db in InvDatabase.list)
		{
			foreach (InvBaseItem item in db.items)
			{
				if (item.name.Equals(partial, System.StringComparison.OrdinalIgnoreCase))
				{
					FindResult fr = new FindResult();
					fr.db = db;
					fr.item = item;
					mResults.Add(fr);
				}
			}
		}

		// Next come partial matches that begin with the specified string
		foreach (InvDatabase db in InvDatabase.list)
		{
			foreach (InvBaseItem item in db.items)
			{
				if (item.name.StartsWith(partial, System.StringComparison.OrdinalIgnoreCase))
				{
					bool exists = false;

					foreach (FindResult res in mResults)
					{
						if (res.item == item)
						{
							exists = true;
							break;
						}
					}

					if (!exists)
					{
						FindResult fr = new FindResult();
						fr.db = db;
						fr.item = item;
						mResults.Add(fr);
					}
				}
			}
		}

		// Other partial matches come last
		foreach (InvDatabase db in InvDatabase.list)
		{
			foreach (InvBaseItem item in db.items)
			{
				if (item.name.ToLower().Contains(partial))
				{
					bool exists = false;

					foreach (FindResult res in mResults)
					{
						if (res.item == item)
						{
							exists = true;
							break;
						}
					}

					if (!exists)
					{
						FindResult fr = new FindResult();
						fr.db = db;
						fr.item = item;
						mResults.Add(fr);
					}
				}
			}
		}
	}
}
