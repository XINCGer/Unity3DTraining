using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipsUI : MonoBehaviour {

    public Text OutlineText;
    public Text ContentText;

    public void UpdateTooltip(string text) {
        OutlineText.text = text;
        ContentText.text = text;
    }
}
