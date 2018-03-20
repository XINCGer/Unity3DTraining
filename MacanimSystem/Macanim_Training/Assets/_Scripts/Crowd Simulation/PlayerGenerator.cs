using UnityEngine;
using System.Collections;

public class PlayerGenerator : MonoBehaviour 
{
	public GameObject dude;
	public GameObject teddy;
	public int showCount = 0;
	public int maxPlayerCount = 50;
	
	static int count = 0;
	static float lastTime = 0;
	private float timeSpan = 0.1f;

	void Start () 
	{
		lastTime = Time.time;
	}

	void Update () 
	{
		if(count < maxPlayerCount)
		{
			bool fired = Input.GetButton("Fire1");
			if((Time.time - lastTime) > timeSpan)
			{
				if(dude != null && !fired)
					Instantiate(dude, Vector3.zero, Quaternion.identity);
				if(teddy != null && fired)
					Instantiate(teddy, Vector3.zero, Quaternion.identity);
				lastTime = Time.time;
				++count;
				showCount = count;
			}
		}
	}
}
