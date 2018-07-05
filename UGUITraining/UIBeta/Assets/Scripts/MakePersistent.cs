using UnityEngine;
using System.Collections;

public class MakePersistent : MonoBehaviour
{
	void Start () 
	{
		DontDestroyOnLoad(this);
	}
}
