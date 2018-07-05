using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IgnoreRaycast : MonoBehaviour, ICanvasRaycastFilter 
{
	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
	{
		return false;
	}
}
