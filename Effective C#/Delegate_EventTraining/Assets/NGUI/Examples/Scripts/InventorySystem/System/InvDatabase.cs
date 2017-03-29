using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Examples/Item Database")]
public class InvDatabase : MonoBehaviour
{
	// Cached list of all available item databases
	static InvDatabase[] mList;
	static bool mIsDirty = true;

	/// <summary>
	/// Retrieves the list of item databases, finding all instances if necessary.
	/// </summary>

	static public InvDatabase[] list
	{
		get
		{
			if (mIsDirty)
			{
				mIsDirty = false;
				mList = NGUITools.FindActive<InvDatabase>();
			}
			return mList;
		}
	}

	/// <summary>
	/// Each database should have a unique 16-bit ID. When the items are saved, database IDs
	/// get combined with item IDs to create 32-bit IDs containing both values.
	/// </summary>

	public int databaseID = 0;

	/// <summary>
	/// List of items in this database.
	/// </summary>

	public List<InvBaseItem> items = new List<InvBaseItem>();

	/// <summary>
	/// UI atlas used for icons.
	/// </summary>

	public UIAtlas iconAtlas;

	/// <summary>
	/// Add this database to the list.
	/// </summary>

	void OnEnable () { mIsDirty = true; }

	/// <summary>
	/// Remove this database from the list.
	/// </summary>

	void OnDisable () { mIsDirty = true; }

	/// <summary>
	/// Find an item by its 16-bit ID.
	/// </summary>

	InvBaseItem GetItem (int id16)
	{
		for (int i = 0, imax = items.Count; i < imax; ++i)
		{
			InvBaseItem item = items[i];
			if (item.id16 == id16) return item;
		}
		return null;
	}

	/// <summary>
	/// Find a database given its ID.
	/// </summary>

	static InvDatabase GetDatabase (int dbID)
	{
		for (int i = 0, imax = list.Length; i < imax; ++i)
		{
			InvDatabase db = list[i];
			if (db.databaseID == dbID) return db;
		}
		return null;
	}

	/// <summary>
	/// Find the specified item given its full 32-bit ID (not to be confused with individual 16-bit item IDs).
	/// </summary>

	static public InvBaseItem FindByID (int id32)
	{
		InvDatabase db = GetDatabase(id32 >> 16);
		return (db != null) ? db.GetItem(id32 & 0xFFFF) : null;
	}

	/// <summary>
	/// Find the item with the specified name.
	/// </summary>

	static public InvBaseItem FindByName (string exact)
	{
		for (int i = 0, imax = list.Length; i < imax; ++i)
		{
			InvDatabase db = list[i];

			for (int b = 0, bmax = db.items.Count; b < bmax; ++b)
			{
				InvBaseItem item = db.items[b];

				if (item.name == exact)
				{
					return item;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Get the full 32-bit ID of the specified item.
	/// Use this to get a list of items on the character that can get saved out to an external database or file.
	/// </summary>

	static public int FindItemID (InvBaseItem item)
	{
		for (int i = 0, imax = list.Length; i < imax; ++i)
		{
			InvDatabase db = list[i];

			if (db.items.Contains(item))
			{
				return (db.databaseID << 16) | item.id16;
			}
		}
		return -1;
	}
}