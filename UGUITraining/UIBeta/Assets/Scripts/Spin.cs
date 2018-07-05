using UnityEngine;

public class Spin : MonoBehaviour
{
	public float rotationsPerSecond = 0.1f;

	void Update ()
	{
		Vector3 euler = transform.localEulerAngles;
		euler.y += rotationsPerSecond * 360f * Time.deltaTime;
		transform.localEulerAngles = euler;
	}
}
