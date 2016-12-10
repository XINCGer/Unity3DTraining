using System;
using UnityEngine;
using System.Collections;

public class MyScript : MonoBehaviour
{
    public Material [] Material;
    private MeshRenderer _meshRenderer;
	// Use this for initialization
	void Start ()
	{
	    _meshRenderer = this.transform.GetComponent<MeshRenderer>();
	}


    public void ExchangeMaterial(int index)
    {
        _meshRenderer.material = Material[index];
    }
}
