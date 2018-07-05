using UnityEditor;
using UnityEngine;

public class AlphaNumericSort : BaseHierarchySort
{
	public override int Compare(GameObject lhs, GameObject rhs)
	{
		if (lhs == rhs) return 0;
		if (lhs == null) return -1;
		if (rhs == null) return 1;

		return EditorUtility.NaturalCompare(lhs.name, rhs.name);
	}
}
