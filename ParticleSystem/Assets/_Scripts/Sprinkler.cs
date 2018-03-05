using UnityEngine;
using System.Collections;

public class Sprinkler : MonoBehaviour
{
	private float heightAboveFloor;
	private ParticleCollisionEvent[][] collisionEvents;
	private GameObject barrel;
	private ParticleSystem[] sprinklers;
	private GameObject floor;
	private Fire fire;
	
	void Awake ()
	{
		barrel = GameObject.FindGameObjectWithTag("FireBarrel");
		fire = barrel.GetComponent<Fire>();
		sprinklers = GetComponentsInChildren<ParticleSystem>();
	}
	
	void Start ()
	{
		heightAboveFloor = transform.position.y;
		collisionEvents = new ParticleCollisionEvent[sprinklers.Length][];
	}

	void OnParticleCollision(GameObject other)
	{
		if(other.tag == "FireBarrel")
		{
			for(int i = 0; i < collisionEvents.Length; i++)
			{
				collisionEvents[i] = new ParticleCollisionEvent[sprinklers[i].GetSafeCollisionEventSize()];
			}
			
			for(int i = 0; i < collisionEvents.Length; i++)
			{
				sprinklers[i].GetCollisionEvents(gameObject, collisionEvents[i]);
			}
			
			for(int i = 0; i < collisionEvents.Length; i++)
			{
				for(int j = 0; j < collisionEvents[i].Length; j++)
				{
					foreach(ParticleHelper ph in fire.particles)
					{
						if(ph.varyAlpha)
							ph.DecreaseAlpha();
						if(ph.varyEmission)
							ph.DecreaseEmission();
						if(ph.varyIntensity)
							ph.DecreaseIntensity();
						if(ph.varyRange)
							ph.DecreaseRange();
					}
				}
			}
		}
	}

	void Update ()
	{
		if(Input.GetMouseButton(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits;
			
			hits = Physics.RaycastAll(ray);
			
			foreach(RaycastHit h in hits)
			{
				if(h.collider.name == "ground")
					transform.position = h.point +  new Vector3(0f, heightAboveFloor, 0f);
			}
			
			if(!sprinklers[0].isPlaying)
			{
				for(int i = 0; i < sprinklers.Length; i++)
				{
					sprinklers[i].Play();
				}
			}
		}
		else
		{
			if(sprinklers[0].isPlaying)
			{
				for(int i = 0; i < sprinklers.Length; i++)
				{
					sprinklers[i].Stop();
				}
			}
		}
	}
}
