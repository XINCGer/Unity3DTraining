using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Displacement/Vortex")]
public class VortexEffect : ImageEffectBase {
	public Vector2  radius = new Vector2(0.4F,0.4F);
	public float    angle = 50;
	public Vector2  center = new Vector2(0.5F, 0.5F);

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		ImageEffects.RenderDistortion (material, source, destination, angle, center, radius);
	}
}
