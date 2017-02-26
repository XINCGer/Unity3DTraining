using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{

    public Text ItemText;
    public Image ItemImage;

    public void UpdateText(string text)
    {
        this.ItemText.text = text;
    }

    public void UpdateImage(Sprite sprite)
    {
        this.ItemImage.sprite = sprite;
    }
}
