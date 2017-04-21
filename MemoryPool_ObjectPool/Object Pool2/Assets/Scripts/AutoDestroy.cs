using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        StartCoroutine(DestroySelf());		
	}

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(3f);
        GameObjectPool.Store(this.gameObject);
    }
}
