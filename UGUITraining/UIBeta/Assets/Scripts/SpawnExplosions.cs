using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpawnExplosions : MonoBehaviour
{
	public GameObject prefab;
	public Camera viewCamera;
	public Vector2 range = new Vector2(300f, 200f);
	public float frequency = 0.25f;
	public float duration = 1f;

	float m_NextSpawn = 0f;

	void Update ()
	{
		float time = Time.time;
		var system = EventSystem.current;
		if (time > m_NextSpawn && system && !system.IsPointerOverGameObject())
		{
			m_NextSpawn = time + frequency;

			Vector3 position = Input.mousePosition;
			position.z = 10f;
			position = GetComponent<Camera>().ScreenToWorldPoint(position);

			GameObject child = Instantiate (prefab) as GameObject;
			child.transform.localPosition = position;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
			Destroy(child, duration);
		}
	}
}
