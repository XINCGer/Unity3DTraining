using UnityEngine;
using System.Collections;

public class NPCName : MonoBehaviour 
{
	public string labelText = "";
	public GUISkin customSkin = null;
	public string styleName = "Box";
	public Camera guiCamera = null;
	public float fadeDistance = 12.0f;
	public float hideDistance = 17.0f;
	public float maxViewAngle = 90.0f;

	void OnGUI ()
	{
		useGUILayout = false;
		
		if (Event.current.type != EventType.Repaint)
			return;
		
		Vector3 worldPosition = GetComponent<Collider>().bounds.center + Vector3.up * GetComponent<Collider>().bounds.size.y * 0.5f;
		Vector3 distance = worldPosition - guiCamera.transform.position;
		float cameraDistance = distance.magnitude;
		
		if (cameraDistance > hideDistance ||
		    Vector3.Angle (guiCamera.transform.forward, distance) > maxViewAngle)
			return;
		
		if (cameraDistance > fadeDistance)
		{
			GUI.color = new Color (1.0f, 1.0f, 1.0f, 1.0f - (cameraDistance - fadeDistance) / (hideDistance - fadeDistance));
		}
		
		Vector2 position = guiCamera.WorldToScreenPoint (worldPosition);
		//print ("beofore : " + position.ToString());
		position = new Vector2 (position.x, Screen.height - position.y);
		//print ("after: " + position.ToString());
		GUI.skin = customSkin;
		string contents = string.IsNullOrEmpty (labelText) ? gameObject.name : labelText;
		Vector2 size = GUI.skin.GetStyle (styleName).CalcSize (new GUIContent (contents));
		
		Rect rect = new Rect (position.x - size.x * 0.5f, position.y - size.y, size.x, size.y);
		GUI.skin.GetStyle (styleName).Draw (rect, contents, false, false, false, false);
		//GUI.skin.GetStyle (styleName).Draw (new Rect(0, 10, 100, 100), "Test", false, false, false, false);
	}
}
