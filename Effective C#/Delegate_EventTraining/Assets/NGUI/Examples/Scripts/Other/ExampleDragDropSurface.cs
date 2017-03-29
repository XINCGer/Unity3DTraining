//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Simple example of an OnDrop event accepting a game object. In this case we check to see if there is a DragDropObject present,
/// and if so -- create its prefab on the surface, then destroy the object.
/// </summary>

[AddComponentMenu("NGUI/Examples/Drag and Drop Surface (Example)")]
public class ExampleDragDropSurface : MonoBehaviour
{
	public bool rotatePlacedObject = false;

	//void OnDrop (GameObject go)
	//{
	//    ExampleDragDropItem ddo = go.GetComponent<ExampleDragDropItem>();

	//    if (ddo != null)
	//    {
	//        GameObject child = NGUITools.AddChild(gameObject, ddo.prefab);

	//        Transform trans = child.transform;
	//        trans.position = UICamera.lastWorldPosition;
	//        if (rotatePlacedObject) trans.rotation = Quaternion.LookRotation(UICamera.lastHit.normal) * Quaternion.Euler(90f, 0f, 0f);
	//        Destroy(go);
	//    }
	//}
}
