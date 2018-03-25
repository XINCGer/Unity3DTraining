using UnityEngine;
using System.Collections;

/// <summary>
/// NPC spawner
/// </summary>
public class Spawner : MonoBehaviour 
{
	public GameObject ToSpawn;		
	public int SpawnCount = 50;	
	private bool m_IsSpawning = false;
	
	int m_CurrentSpawnCount;	
	
	public void Update()
	{		
		if(m_IsSpawning && m_CurrentSpawnCount <  SpawnCount)
		{
			for(int i = 0 ; i < 50 ; i++) // try 50 times. Brute force approach, randomly try to spawn and make sure its in the Spawnable zone. 
			{
				Vector3 position = transform.position + Random.insideUnitSphere * 20;
				position.y = 0;
							
				RaycastHit hitInfo;
				if(Physics.Raycast(position + new Vector3(0,1,0), Vector3.down,out hitInfo,10) && hitInfo.collider.tag == "Spawnable")
				{
					GameObject newNPC = Instantiate(ToSpawn, position ,  Quaternion.Euler(0, Random.value*180, 0)) as GameObject;	
					newNPC.transform.parent = transform.GetChild(0).transform;									
					newNPC.SetActive(true);
					m_CurrentSpawnCount++;
					return;
				}
			}
		}
	}
		
	public void BeginSpawning()
	{
		m_IsSpawning = true;		
	}
}
