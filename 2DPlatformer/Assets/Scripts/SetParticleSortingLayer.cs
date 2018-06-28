using UnityEngine;
using System.Collections;

public class SetParticleSortingLayer : MonoBehaviour
{
	public string sortingLayerName;		// The name of the sorting layer the particles should be set to.


	void Start ()
	{
		// Set the sorting layer of the particle system.
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayerName;
	}
}
