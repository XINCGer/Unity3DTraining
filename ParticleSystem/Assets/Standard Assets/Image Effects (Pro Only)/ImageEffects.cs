using UnityEngine;
//using System;

/// A Utility class for performing various image based rendering tasks.
[AddComponentMenu("")]
public class ImageEffects
{
    public static void RenderDistortion(Material material, RenderTexture source, RenderTexture destination, float angle, Vector2 center, Vector2 radius)
    {
        bool invertY = source.texelSize.y < 0.0f;
        if (invertY)
        {
            center.y = 1.0f - center.y;
            angle = -angle;
        }

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);

        material.SetMatrix("_RotationMatrix", rotationMatrix);
        material.SetVector("_CenterRadius", new Vector4(center.x, center.y, radius.x, radius.y));
        material.SetFloat("_Angle", angle * Mathf.Deg2Rad);

        Graphics.Blit(source, destination, material);
    }
    
	[System.Obsolete("Use Graphics.Blit(source,dest) instead")]
	public static void Blit(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, dest);
	}

	[System.Obsolete("Use Graphics.Blit(source, destination, material) instead")]
	public static void BlitWithMaterial(Material material, RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, dest, material);
	}        
}