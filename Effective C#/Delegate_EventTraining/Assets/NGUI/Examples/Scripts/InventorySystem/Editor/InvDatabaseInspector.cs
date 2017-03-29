using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit Inventory Databases.
/// </summary>

[CustomEditor(typeof(InvDatabase))]
public class InvDatabaseInspector : Editor
{
	static int mIndex = 0;

	bool mConfirmDelete = false;

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite (Texture2D tex, Rect sprite, Material mat) { return DrawSprite(tex, sprite, mat, true, 0); }

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite (Texture2D tex, Rect sprite, Material mat, bool addPadding)
	{
		return DrawSprite(tex, sprite, mat, addPadding, 0);
	}

	/// <summary>
	/// Draw an enlarged sprite within the specified texture atlas.
	/// </summary>

	public Rect DrawSprite (Texture2D tex, Rect sprite, Material mat, bool addPadding, int maxSize)
	{
		float paddingX = addPadding ? 4f / tex.width : 0f;
		float paddingY = addPadding ? 4f / tex.height : 0f;
		float ratio = (sprite.height + paddingY) / (sprite.width + paddingX);

		ratio *= (float)tex.height / tex.width;

		// Draw the checkered background
		Color c = GUI.color;
		Rect rect = NGUIEditorTools.DrawBackground(tex, ratio);
		GUI.color = c;

		if (maxSize > 0)
		{
			float dim = maxSize / Mathf.Max(rect.width, rect.height);
			rect.width *= dim;
			rect.height *= dim;
		}

		// We only want to draw into this rectangle
		if (Event.current.type == EventType.Repaint)
		{
			if (mat == null)
			{
				GUI.DrawTextureWithTexCoords(rect, tex, sprite);
			}
			else
			{
				// NOTE: DrawPreviewTexture doesn't seem to support BeginGroup-based clipping
				// when a custom material is specified. It seems to be a bug in Unity.
				// Passing 'null' for the material or omitting the parameter clips as expected.
				UnityEditor.EditorGUI.DrawPreviewTexture(sprite, tex, mat);
				//UnityEditor.EditorGUI.DrawPreviewTexture(drawRect, tex);
				//GUI.DrawTexture(drawRect, tex);
			}
			rect = new Rect(sprite.x + rect.x, sprite.y + rect.y, sprite.width, sprite.height);
		}
		return rect;
	}

	/// <summary>
	/// Helper function that sets the index to the index of the specified item.
	/// </summary>

	public static void SelectIndex (InvDatabase db, InvBaseItem item)
	{
		mIndex = 0;

		foreach (InvBaseItem i in db.items)
		{
			if (i == item) break;
			++mIndex;
		}
	}

	/// <summary>
	/// Draw the inspector widget.
	/// </summary>

	public override void OnInspectorGUI ()
	{
		NGUIEditorTools.SetLabelWidth(80f);
		InvDatabase db = target as InvDatabase;
		NGUIEditorTools.DrawSeparator();

		InvBaseItem item = null;

		if (db.items == null || db.items.Count == 0)
		{
			mIndex = 0;
		}
		else
		{
			mIndex = Mathf.Clamp(mIndex, 0, db.items.Count - 1);
			item = db.items[mIndex];
		}

		if (mConfirmDelete)
		{
			// Show the confirmation dialog
			GUILayout.Label("Are you sure you want to delete '" + item.name + "'?");
			NGUIEditorTools.DrawSeparator();

			GUILayout.BeginHorizontal();
			{
				GUI.backgroundColor = Color.green;
				if (GUILayout.Button("Cancel")) mConfirmDelete = false;
				GUI.backgroundColor = Color.red;

				if (GUILayout.Button("Delete"))
				{
					NGUIEditorTools.RegisterUndo("Delete Inventory Item", db);
					db.items.RemoveAt(mIndex);
					mConfirmDelete = false;
				}
				GUI.backgroundColor = Color.white;
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			// Database icon atlas
			UIAtlas atlas = EditorGUILayout.ObjectField("Icon Atlas", db.iconAtlas, typeof(UIAtlas), false) as UIAtlas;

			if (atlas != db.iconAtlas)
			{
				NGUIEditorTools.RegisterUndo("Databse Atlas change", db);
				db.iconAtlas = atlas;
				foreach (InvBaseItem i in db.items) i.iconAtlas = atlas;
			}

			// Database ID
			int dbID = EditorGUILayout.IntField("Database ID", db.databaseID);

			if (dbID != db.databaseID)
			{
				NGUIEditorTools.RegisterUndo("Database ID change", db);
				db.databaseID = dbID;
			}

			// "New" button
			GUI.backgroundColor = Color.green;

			if (GUILayout.Button("New Item"))
			{
				NGUIEditorTools.RegisterUndo("Add Inventory Item", db);

				InvBaseItem bi = new InvBaseItem();
				bi.iconAtlas = db.iconAtlas;
				bi.id16 = (db.items.Count > 0) ? db.items[db.items.Count - 1].id16 + 1 : 0;
				db.items.Add(bi);
				mIndex = db.items.Count - 1;

				if (item != null)
				{
					bi.name = "Copy of " + item.name;
					bi.description = item.description;
					bi.slot = item.slot;
					bi.color = item.color;
					bi.iconName = item.iconName;
					bi.attachment = item.attachment;
					bi.minItemLevel = item.minItemLevel;
					bi.maxItemLevel = item.maxItemLevel;

					foreach (InvStat stat in item.stats)
					{
						InvStat copy = new InvStat();
						copy.id = stat.id;
						copy.amount = stat.amount;
						copy.modifier = stat.modifier;
						bi.stats.Add(copy);
					}
				}
				else
				{
					bi.name = "New Item";
					bi.description = "Item Description";
				}

				item = bi;
			}
			GUI.backgroundColor = Color.white;

			if (item != null)
			{
				NGUIEditorTools.DrawSeparator();

				// Navigation section
				GUILayout.BeginHorizontal();
				{
					if (mIndex == 0) GUI.color = Color.grey;
					if (GUILayout.Button("<<")) { mConfirmDelete = false; --mIndex; }
					GUI.color = Color.white;
					mIndex = EditorGUILayout.IntField(mIndex + 1, GUILayout.Width(40f)) - 1;
					GUILayout.Label("/ " + db.items.Count, GUILayout.Width(40f));
					if (mIndex + 1 == db.items.Count) GUI.color = Color.grey;
					if (GUILayout.Button(">>")) { mConfirmDelete = false; ++mIndex; }
					GUI.color = Color.white;
				}
				GUILayout.EndHorizontal();

				NGUIEditorTools.DrawSeparator();

				// Item name and delete item button
				GUILayout.BeginHorizontal();
				{
					string itemName = EditorGUILayout.TextField("Item Name", item.name);

					GUI.backgroundColor = Color.red;

					if (GUILayout.Button("Delete", GUILayout.Width(55f)))
					{
						mConfirmDelete = true;
					}
					GUI.backgroundColor = Color.white;

					if (!itemName.Equals(item.name))
					{
						NGUIEditorTools.RegisterUndo("Rename Item", db);
						item.name = itemName;
					}
				}
				GUILayout.EndHorizontal();

				string itemDesc = GUILayout.TextArea(item.description, 200, GUILayout.Height(100f));
				InvBaseItem.Slot slot = (InvBaseItem.Slot)EditorGUILayout.EnumPopup("Slot", item.slot);
				string iconName = "";
				float iconSize = 64f;
				bool drawIcon = false;
				float extraSpace = 0f;

				if (item.iconAtlas != null)
				{
					BetterList<string> sprites = item.iconAtlas.GetListOfSprites();
					sprites.Insert(0, "<None>");

					int index = 0;
					string spriteName = (item.iconName != null) ? item.iconName : sprites[0];

					// We need to find the sprite in order to have it selected
					if (!string.IsNullOrEmpty(spriteName))
					{
						for (int i = 1; i < sprites.size; ++i)
						{
							if (spriteName.Equals(sprites[i], System.StringComparison.OrdinalIgnoreCase))
							{
								index = i;
								break;
							}
						}
					}

					// Draw the sprite selection popup
					index = EditorGUILayout.Popup("Icon", index, sprites.ToArray());
					UISpriteData sprite = (index > 0) ? item.iconAtlas.GetSprite(sprites[index]) : null;

					if (sprite != null)
					{
						iconName = sprite.name;

						Material mat = item.iconAtlas.spriteMaterial;

						if (mat != null)
						{
							Texture2D tex = mat.mainTexture as Texture2D;

							if (tex != null)
							{
								drawIcon = true;
								Rect rect = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
								rect = NGUIMath.ConvertToTexCoords(rect, tex.width, tex.height);

								GUILayout.Space(4f);
								GUILayout.BeginHorizontal();
								{
									GUILayout.Space(Screen.width - iconSize);
									DrawSprite(tex, rect, null, false);
								}
								GUILayout.EndHorizontal();

								extraSpace = iconSize * (float)sprite.height / sprite.width;
							}
						}
					}
				}

				// Item level range
				GUILayout.BeginHorizontal();
				GUILayout.Label("Level Range", GUILayout.Width(77f));
				int min = EditorGUILayout.IntField(item.minItemLevel, GUILayout.MinWidth(40f));
				int max = EditorGUILayout.IntField(item.maxItemLevel, GUILayout.MinWidth(40f));
				if (drawIcon) GUILayout.Space(iconSize);
				GUILayout.EndHorizontal();

				// Game Object attachment field, left of the icon
				GUILayout.BeginHorizontal();
				GameObject go = (GameObject)EditorGUILayout.ObjectField("Attachment", item.attachment, typeof(GameObject), false);
				if (drawIcon) GUILayout.Space(iconSize);
				GUILayout.EndHorizontal();

				// Color tint field, left of the icon
				GUILayout.BeginHorizontal();
				Color color = EditorGUILayout.ColorField("Color", item.color);
				if (drawIcon) GUILayout.Space(iconSize);
				GUILayout.EndHorizontal();

				// Calculate the extra spacing necessary for the icon to show up properly and not overlap anything
				if (drawIcon)
				{
					extraSpace = Mathf.Max(0f, extraSpace - 60f);
					GUILayout.Space(extraSpace);
				}

				// Item stats
				NGUIEditorTools.DrawSeparator();

				if (item.stats != null)
				{
					for (int i = 0; i < item.stats.Count; ++i)
					{
						InvStat stat = item.stats[i];

						GUILayout.BeginHorizontal();
						{
							InvStat.Identifier iden = (InvStat.Identifier)EditorGUILayout.EnumPopup(stat.id, GUILayout.Width(80f));

							// Color the field red if it's negative, green if it's positive
							if (stat.amount > 0) GUI.backgroundColor = Color.green;
							else if (stat.amount < 0) GUI.backgroundColor = Color.red;
							int amount = EditorGUILayout.IntField(stat.amount, GUILayout.Width(40f));
							GUI.backgroundColor = Color.white;

							InvStat.Modifier mod = (InvStat.Modifier)EditorGUILayout.EnumPopup(stat.modifier);

							GUI.backgroundColor = Color.red;
							if (GUILayout.Button("X", GUILayout.Width(20f)))
							{
								NGUIEditorTools.RegisterUndo("Delete Item Stat", db);
								item.stats.RemoveAt(i);
								--i;
							}
							else if (iden != stat.id || amount != stat.amount || mod != stat.modifier)
							{
								NGUIEditorTools.RegisterUndo("Item Stats", db);
								stat.id = iden;
								stat.amount = amount;
								stat.modifier = mod;
							}
							GUI.backgroundColor = Color.white;
						}
						GUILayout.EndHorizontal();
					}
				}

				if (GUILayout.Button("Add Stat", GUILayout.Width(80f)))
				{
					NGUIEditorTools.RegisterUndo("Add Item Stat", db);
					InvStat stat = new InvStat();
					stat.id = InvStat.Identifier.Armor;
					item.stats.Add(stat);
				}

				// Save all values
				if (!itemDesc.Equals(item.description) ||
					slot	!= item.slot ||
					go		!= item.attachment ||
					color	!= item.color ||
					min		!= item.minItemLevel ||
					max		!= item.maxItemLevel ||
					!iconName.Equals(item.iconName))
				{
					NGUIEditorTools.RegisterUndo("Item Properties", db);
					item.description = itemDesc;
					item.slot = slot;
					item.attachment = go;
					item.color = color;
					item.iconName = iconName;
					item.minItemLevel = min;
					item.maxItemLevel = max;
				}
			}
		}
	}
}
