using UnityEngine;
using System.Collections;

[System.Serializable]
public class ParticleHelper 
{
	public ParticleSystem part;
	public Light light;

	public bool varyAlpha;
	public bool varyEmission;
	public bool varyIntensity;
	public bool varyRange;
	
	public float minAlpha;
	public float maxAlpha;
	public float alphaIncreaseRate;
	public float alphaDecreaseRate;
	public float alphaVariation;
	
	public float minEmission;
	public float maxEmission;
	public float emissionIncreaseRate;
	public float emissionDecreaseRate;
	public float emissionVariation;
	
	public float minIntensity;
	public float maxIntensity;
	public float intensityIncreaseRate;
	public float intensityDecreaseRate;
	public float intensityVariation;
	
	public float minRange;
	public float maxRange;
	public float rangeIncreaseRate;
	public float rangeDecreaseRate;
	public float rangeVariation;

	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void IncreaseAlpha ()
	{
		if(part.startColor.a < maxAlpha)
		{
			Color adjustedColour = part.startColor;
			adjustedColour.a += alphaIncreaseRate * Time.deltaTime;
			adjustedColour.a += Random.Range(0f, alphaVariation);
			part.startColor = adjustedColour;
		}
	}

	public void DecreaseAlpha ()
	{
		if(part.startColor.a > minAlpha)
		{
			Color adjustedColour = part.startColor;
			adjustedColour.a -= alphaDecreaseRate * Time.deltaTime;
			part.startColor = adjustedColour;
		}
	}

	public void IncreaseEmission ()
	{
		if(part.emissionRate < maxEmission)
		{
			float emissionRate = part.emissionRate;
			emissionRate += emissionIncreaseRate * Time.deltaTime;
			emissionRate += Random.Range(0f, emissionVariation);
			part.emissionRate = emissionRate;
		}
	}

	public void DecreaseEmission ()
	{
		if(part.emissionRate > minEmission)
			part.emissionRate -= emissionDecreaseRate * Time.deltaTime;
	}

	public void IncreaseIntensity ()
	{
		if(light.intensity < maxIntensity)
		{
			float intensity = light.intensity;
			intensity += intensityIncreaseRate * Time.deltaTime;
			intensity += Random.Range(0f, intensityVariation);
			light.intensity = intensity;
		}
	}
	
	public void DecreaseIntensity ()
	{
		if(light.intensity > minIntensity)
			light.intensity -= intensityDecreaseRate * Time.deltaTime;
	}

	public void IncreaseRange ()
	{
		if(light.range < maxRange)
		{
			float range = light.range;
			range += rangeIncreaseRate * Time.deltaTime;
			range += Random.Range(0f, rangeVariation);
			light.range = range;
		}
	}

	public void DecreaseRange ()
	{
		if(light.range > minRange)
			light.range -= rangeDecreaseRate * Time.deltaTime;
	}
}
