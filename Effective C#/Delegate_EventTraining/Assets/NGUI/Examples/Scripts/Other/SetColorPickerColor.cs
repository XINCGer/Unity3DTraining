using UnityEngine;

[RequireComponent(typeof(UIWidget))]
public class SetColorPickerColor : MonoBehaviour
{
	[System.NonSerialized] UIWidget mWidget;

	public void SetToCurrent ()
	{
		if (mWidget == null) mWidget = GetComponent<UIWidget>();
		if (UIColorPicker.current != null)
			mWidget.color = UIColorPicker.current.value;
	}
}
