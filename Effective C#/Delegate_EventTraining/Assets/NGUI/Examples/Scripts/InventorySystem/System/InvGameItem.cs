using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Since it would be incredibly tedious to create thousands of unique items by hand, a simple solution is needed.
/// Separating items into 2 parts is that solution. Base item contains stats that the item would have if it was max
/// level. All base items are created with their stats at max level. Game item, the second item class, has an
/// effective item level which is used to calculate effective item stats. Game items can be generated with a random
/// level (clamped within base item's min/max level range), and with random quality affecting the item's stats.
/// </summary>

[System.Serializable]
public class InvGameItem
{
	public enum Quality
	{
		Broken,
		Cursed,
		Damaged,
		Worn,
		Sturdy,			// Normal quality
		Polished,
		Improved,
		Crafted,
		Superior,
		Enchanted,
		Epic,
		Legendary,
		_LastDoNotUse,	// Flash export doesn't support Enum.GetNames :(
	}

	// ID of the base item used to create this game item
	[SerializeField] int mBaseItemID = 0;

	/// <summary>
	/// Item quality -- applies a penalty or bonus to all base stats.
	/// </summary>

	public Quality quality = Quality.Sturdy;

	/// <summary>
	/// Item's effective level.
	/// </summary>

	public int itemLevel = 1;

	// Cached for speed
	InvBaseItem mBaseItem;

	/// <summary>
	/// ID of the base item used to create this one.
	/// </summary>

	public int baseItemID { get { return mBaseItemID; } }

	/// <summary>
	/// Base item used by this game item.
	/// </summary>

	public InvBaseItem baseItem
	{
		get
		{
			if (mBaseItem == null)
			{
				mBaseItem = InvDatabase.FindByID(baseItemID);
			}
			return mBaseItem;
		}
	}

	/// <summary>
	/// Game item's name should prefix the quality
	/// </summary>

	public string name
	{
		get
		{
			if (baseItem == null) return null;
			return quality.ToString() + " " + baseItem.name;
		}
	}

	/// <summary>
	/// Put your formula for calculating the item stat modifier here.
	/// Simplest formula -- scale it with quality and item level.
	/// Since all stats on base items are specified at max item level,
	/// calculating the effective multiplier is as simple as itemLevel/maxLevel.
	/// </summary>

	public float statMultiplier
	{
		get
		{
			float mult = 0f;

			switch (quality)
			{
				case Quality.Cursed:	mult = -1f;		break;
				case Quality.Broken:	mult = 0f;		break;
				case Quality.Damaged:	mult = 0.25f;	break;
				case Quality.Worn:		mult = 0.9f;	break;
				case Quality.Sturdy:	mult = 1f;		break;
				case Quality.Polished:	mult = 1.1f;	break;
				case Quality.Improved:	mult = 1.25f;	break;
				case Quality.Crafted:	mult = 1.5f;	break;
				case Quality.Superior:	mult = 1.75f;	break;
				case Quality.Enchanted:	mult = 2f;		break;
				case Quality.Epic:		mult = 2.5f;	break;
				case Quality.Legendary:	mult = 3f;		break;
			}

			// Take item's level into account
			float linear = itemLevel / 50f;

			// Add a curve for more interesting results
			mult *= Mathf.Lerp(linear, linear * linear, 0.5f);
			return mult;
		}
	}

	/// <summary>
	/// Item's color based on quality. You will likely want to change this to your own colors.
	/// </summary>

	public Color color
	{
		get
		{
			Color c = Color.white;

			switch (quality)
			{
				case Quality.Cursed:	c = Color.red; break;
				case Quality.Broken:	c = new Color(0.4f, 0.2f, 0.2f); break;
				case Quality.Damaged:	c = new Color(0.4f, 0.4f, 0.4f); break;
				case Quality.Worn:		c = new Color(0.7f, 0.7f, 0.7f); break;
				case Quality.Sturdy:	c = new Color(1.0f, 1.0f, 1.0f); break;
				case Quality.Polished:	c = NGUIMath.HexToColor(0xe0ffbeff); break;
				case Quality.Improved:	c = NGUIMath.HexToColor(0x93d749ff); break;
				case Quality.Crafted:	c = NGUIMath.HexToColor(0x4eff00ff); break;
				case Quality.Superior:	c = NGUIMath.HexToColor(0x00baffff); break;
				case Quality.Enchanted: c = NGUIMath.HexToColor(0x7376fdff); break;
				case Quality.Epic:		c = NGUIMath.HexToColor(0x9600ffff); break;
				case Quality.Legendary: c = NGUIMath.HexToColor(0xff9000ff); break;
			}
			return c;
		}
	}

	/// <summary>
	/// Create a game item with the specified ID.
	/// </summary>

	public InvGameItem (int id) { mBaseItemID = id; }

	/// <summary>
	/// Create a game item with the specified ID and base item.
	/// </summary>

	public InvGameItem (int id, InvBaseItem bi) { mBaseItemID = id; mBaseItem = bi; }

	/// <summary>
	/// Calculate and return the list of effective stats based on item level and quality.
	/// </summary>

	public List<InvStat> CalculateStats ()
	{
		List<InvStat> stats = new List<InvStat>();

		if (baseItem != null)
		{
			float mult = statMultiplier;
			List<InvStat> baseStats = baseItem.stats;

			for (int i = 0, imax = baseStats.Count; i < imax; ++i)
			{
				InvStat bs = baseStats[i];
				int amount = Mathf.RoundToInt(mult * bs.amount);
				if (amount == 0) continue;

				bool found = false;

				for (int b = 0, bmax = stats.Count; b < bmax; ++b)
				{
					InvStat s = stats[b];

					if (s.id == bs.id && s.modifier == bs.modifier)
					{
						s.amount += amount;
						found = true;
						break;
					}
				}

				if (!found)
				{
					InvStat stat = new InvStat();
					stat.id = bs.id;
					stat.amount = amount;
					stat.modifier = bs.modifier;
					stats.Add(stat);
				}
			}

			// This would be the place to determine if it's a weapon or armor and sort stats accordingly
			stats.Sort(InvStat.CompareArmor);
		}
		return stats;
	}
}