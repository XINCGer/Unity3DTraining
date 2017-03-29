using UnityEngine;

[AddComponentMenu("NGUI/Examples/Item Attachment Point")]
public class InvAttachmentPoint : MonoBehaviour
{
	/// <summary>
	/// Item slot that this attachment point covers.
	/// </summary>

	public InvBaseItem.Slot slot;

	GameObject mPrefab;
	GameObject mChild;

	/// <summary>
	/// Attach an instance of the specified game object.
	/// </summary>

	public GameObject Attach (GameObject prefab)
	{
		if (mPrefab != prefab)
		{
			mPrefab = prefab;

			// Remove the previous child
			if (mChild != null) Destroy(mChild);

			// If we have something to create, let's do so now
			if (mPrefab != null)
			{
				// Create a new instance of the game object
				Transform t = transform;
				mChild = Instantiate(mPrefab, t.position, t.rotation) as GameObject;

				// Parent the child to this object
				Transform ct = mChild.transform;
				ct.parent = t;

				// Reset the pos/rot/scale, just in case
				ct.localPosition = Vector3.zero;
				ct.localRotation = Quaternion.identity;
				ct.localScale = Vector3.one;
			}
		}
		return mChild;
	}
}