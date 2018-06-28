using UnityEngine;
using System.Collections;

public class BackgroundPropSpawner : MonoBehaviour
{
	public Rigidbody2D backgroundProp;		// The prop to be instantiated.
	public float leftSpawnPosX;				// The x coordinate of position if it's instantiated on the left.
	public float rightSpawnPosX;			// The x coordinate of position if it's instantiated on the right.
	public float minSpawnPosY;				// The lowest possible y coordinate of position.
	public float maxSpawnPosY;				// The highest possible y coordinate of position.
	public float minTimeBetweenSpawns;		// The shortest possible time between spawns.
	public float maxTimeBetweenSpawns;		// The longest possible time between spawns.
	public float minSpeed;					// The lowest possible speed of the prop.
	public float maxSpeed;					// The highest possible speeed of the prop.

	void Start ()
	{
		// Set the random seed so it's not the same each game.
		Random.seed = System.DateTime.Today.Millisecond;

		// Start the Spawn coroutine.
		StartCoroutine("Spawn");
	}


	IEnumerator Spawn ()
	{
		// Create a random wait time before the prop is instantiated.
		float waitTime = Random.Range(minTimeBetweenSpawns, maxTimeBetweenSpawns);

		// Wait for the designated period.
		yield return new WaitForSeconds(waitTime);

		// Randomly decide whether the prop should face left or right.
		bool facingLeft = Random.Range(0,2) == 0;

		// If the prop is facing left, it should start on the right hand side, otherwise it should start on the left.
		float posX = facingLeft ? rightSpawnPosX : leftSpawnPosX;

		// Create a random y coordinate for the prop.
		float posY = Random.Range(minSpawnPosY, maxSpawnPosY);

		// Set the position the prop should spawn at.
		Vector3 spawnPos = new Vector3(posX, posY, transform.position.z);

		// Instantiate the prop at the desired position.
		Rigidbody2D propInstance = Instantiate(backgroundProp, spawnPos, Quaternion.identity) as Rigidbody2D;

		// The sprites for the props all face left.  Therefore, if the prop should be facing right...
		if(!facingLeft)
		{
			// ... flip the scale in the x axis.
			Vector3 scale = propInstance.transform.localScale;
			scale.x *= -1;
			propInstance.transform.localScale = scale;
		}

		// Create a random speed.
		float speed = Random.Range(minSpeed, maxSpeed);

		// These speeds would naturally move the prop right, so if it's facing left, multiply the speed by -1.
		speed *= facingLeft ? -1f : 1f;

		// Set the prop's velocity to this speed in the x axis.
		propInstance.velocity = new Vector2(speed, 0);

		// Restart the coroutine to spawn another prop.
		StartCoroutine(Spawn());

		// While the prop exists...
		while(propInstance != null)
		{
			// ... and if it's facing left...
			if(facingLeft)
			{
				// ... and if it's beyond the left spawn position...
				if(propInstance.transform.position.x < leftSpawnPosX - 0.5f)
					// ... destroy the prop.
					Destroy(propInstance.gameObject);
			}
			else
			{
				// Otherwise, if the prop is facing right and it's beyond the right spawn position...
				if(propInstance.transform.position.x > rightSpawnPosX + 0.5f)
					// ... destroy the prop.
					Destroy(propInstance.gameObject);
			}

			// Return to this point after the next update.
			yield return null;
		}
	}
}
