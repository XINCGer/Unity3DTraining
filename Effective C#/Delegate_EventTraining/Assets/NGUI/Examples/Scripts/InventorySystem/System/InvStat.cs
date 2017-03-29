using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inventory System statistic
/// </summary>

[System.Serializable]
public class InvStat
{
	/// <summary>
	/// Customize this enum with statistics appropriate for your own game
	/// </summary>

	public enum Identifier
	{
		Strength,
		Constitution,
		Agility,
		Intelligence,
		Damage,
		Crit,
		Armor,
		Health,
		Mana,
		Other,
	}

	/// <summary>
	/// Formula for final stats: [sum of raw amounts] * (1 + [sum of percent amounts])
	/// </summary>

	public enum Modifier
	{
		Added,
		Percent,
	}

	public Identifier id;
	public Modifier modifier;
	public int amount;

	/// <summary>
	/// Get the localized name of the stat.
	/// </summary>

	static public string GetName (Identifier i)
	{
		return i.ToString();
	}

	/// <summary>
	/// Get the localized stat's description -- adjust this to fit your own stats.
	/// </summary>

	static public string GetDescription (Identifier i)
	{
		switch (i)
		{
			case Identifier.Strength:		return "Strength increases melee damage";
			case Identifier.Constitution:	return "Constitution increases health";
			case Identifier.Agility:		return "Agility increases armor";
			case Identifier.Intelligence:	return "Intelligence increases mana";
			case Identifier.Damage:			return "Damage adds to the amount of damage done in combat";
			case Identifier.Crit:			return "Crit increases the chance of landing a critical strike";
			case Identifier.Armor:			return "Armor protects from damage";
			case Identifier.Health:			return "Health prolongs life";
			case Identifier.Mana:			return "Mana increases the number of spells that can be cast";
		}
		return null;
	}

	/// <summary>
	/// Comparison function for sorting armor. Armor value will show up first, followed by damage.
	/// </summary>

	static public int CompareArmor (InvStat a, InvStat b)
	{
		int ia = (int)a.id;
		int ib = (int)b.id;
		
		if		(a.id == Identifier.Armor)	ia -= 10000;
		else if (a.id == Identifier.Damage) ia -= 5000;
		if		(b.id == Identifier.Armor)	ib -= 10000;
		else if (b.id == Identifier.Damage) ib -= 5000;
		
		if (a.amount < 0) ia += 1000;
		if (b.amount < 0) ib += 1000;
		
		if (a.modifier == Modifier.Percent) ia += 100;
		if (b.modifier == Modifier.Percent) ib += 100;
		
		// Not using ia.CompareTo(ib) here because Flash export doesn't understand that
		if (ia < ib) return -1;
		if (ia > ib) return 1;
		return 0;
	}

	/// <summary>
	/// Comparison function for sorting weapons. Damage value will show up first, followed by armor.
	/// </summary>

	static public int CompareWeapon (InvStat a, InvStat b)
	{
		int ia = (int)a.id;
		int ib = (int)b.id;

		if		(a.id == Identifier.Damage) ia -= 10000;
		else if (a.id == Identifier.Armor)  ia -= 5000;
		if		(b.id == Identifier.Damage) ib -= 10000;
		else if (b.id == Identifier.Armor)  ib -= 5000;

		if (a.amount < 0) ia += 1000;
		if (b.amount < 0) ib += 1000;
		
		if (a.modifier == Modifier.Percent) ia += 100;
		if (b.modifier == Modifier.Percent) ib += 100;
		
		if (ia < ib) return -1;
		if (ia > ib) return 1;
		return 0;
	}
}