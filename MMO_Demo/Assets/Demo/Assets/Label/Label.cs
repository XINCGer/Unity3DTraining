using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Collider))]
public class Label : MonoBehaviour
{
	public string labelText = "";
		// The text rendered in the label.
	public GUISkin customSkin = null;
		// The skin containing the style used to render the label (leave as null to use the default skin)
	public string styleName = "Box";
		// The style used to render the label. Must be available in the used skin.
	public Camera guiCamera = null;
		// The camera used to display the GUI. Used for coordinate and distance calculations.
	public float fadeDistance = 30.0f, hideDistance = 35.0f;
		// Specifies when the label should start fading and when it should hide
	public float maxViewAngle = 90.0f;
		// Specifies at which angle to the camera forward vector, the label should no longer render
	
	
	void Reset ()
	// Fallback for the camera reference
	{
		if (guiCamera == null)
		{
			guiCamera = Camera.main;
			maxViewAngle = guiCamera.fieldOfView * 0.5f;
		}
	}
	
	
	public void SetLabel (string label)
	// Handle SetLabel messages sent to the GO
	{
		labelText = label;
	}
	
	
	void OnGUI ()
	{
		useGUILayout = false;
			// We're not using GUILayout, so don't spend processing on it
		
		if (Event.current.type != EventType.Repaint)
		// We are only interested in repaint events
		{
			return;
		}
		
		Vector3 worldPosition = GetComponent<Collider>().bounds.center + Vector3.up * GetComponent<Collider>().bounds.size.y * 0.5f;
			// Place the label on top of the collider
		float cameraDistance = (worldPosition - guiCamera.transform.position).magnitude;
		
		if (
			cameraDistance > hideDistance ||
			Vector3.Angle (
				guiCamera.transform.forward,
				worldPosition - guiCamera.transform.position
			) >
			maxViewAngle
		)
		// If the world position is outside of the field of view or further away than hideDistance, don't render the label
		{
			return;
		}
		
		if (cameraDistance > fadeDistance)
		// If the distance to the label position is greater than the fade distance, apply the needed fade to the label
		{
			GUI.color = new Color (
				1.0f,
				1.0f,
				1.0f,
				1.0f - (cameraDistance - fadeDistance) / (hideDistance - fadeDistance)
			);
		}
		
		Vector2 position = guiCamera.WorldToScreenPoint (worldPosition);
		position = new Vector2 (position.x, Screen.height - position.y);
			// Get the GUI space position
			
		GUI.skin = customSkin;
			// Set the custom skin. If no custom skin is set (null), Unity will use the default skin
		
		string contents = string.IsNullOrEmpty (labelText) ? gameObject.name : labelText;
		
		Vector2 size = GUI.skin.GetStyle (styleName).CalcSize (new GUIContent (contents));
			// Get the content size with the selected style
		
		Rect rect = new Rect (position.x - size.x * 0.5f, position.y - size.y, size.x, size.y);
			// Construct a rect based on the calculated position and size
		
		GUI.skin.GetStyle (styleName).Draw (rect, contents, false, false, false, false);
			// Draw the label with the selected style
	}
}
