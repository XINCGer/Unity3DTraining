using UnityEngine;
using System.Collections;


/// <summary>
/// Utilities to convert Joystiq input to worldspace ( based in main camera) 
/// and to convert worldspace to Speed and Direction
/// </summary>
public class JoystickToWorld 
{	
	public static Vector3 ConvertJoystickToWorldSpace()
	{		
		Vector3  direction;	        
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");		
        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);                        		        
        direction = Camera.main.transform.rotation * stickDirection; // Converts joystick input in Worldspace coordinates
		direction.y = 0; // Kill Z
		direction.Normalize();		        
	
		return direction;
	}
	
	
    public static void ComputeSpeedDirection(Transform root, ref float speed, ref float direction)
    {
        Vector3 worldDirection = ConvertJoystickToWorldSpace();
        
        speed = Mathf.Clamp(worldDirection.magnitude, 0, 1);
        if (speed > 0.01f) // dead zone
        {
            Vector3 axis = Vector3.Cross(root.forward, worldDirection);
            direction = Vector3.Angle(root.forward, worldDirection) / 180.0f * (axis.y < 0 ? -1 : 1);
        }
        else
        {
            direction = 0.0f; 
        }
    }    
}
