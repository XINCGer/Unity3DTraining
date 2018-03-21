using UnityEngine;
using System.Collections;

public class Spawner: MonoBehaviour {

    public GameObject prefabA;
    public GameObject prefabB;
    static int count = 0;
    static float lastTime = 0;
    public int showCount = 0;


	void OnGUI()
	{

		GUILayout.Label("Spawns up to 200 characters");
		GUILayout.Label("Press fire button to spawn Teddy!");		
	}

    void Start()
    {
        lastTime = Time.time;
    }

    void Update()
    {
        if(count < 200)
        {
            bool alt = Input.GetButton("Fire1");

            if (Time.time - lastTime > 0.1f)
            {
                if(prefabA != null && !alt) Instantiate(prefabA, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0));
                if(prefabB != null && alt) Instantiate(prefabB, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));
                lastTime = Time.time;
                count++;
                showCount = count;
            }
        }
    }
}
