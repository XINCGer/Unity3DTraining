using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 业务逻辑View
/// </summary>
public partial class UILoginPanel
{
    protected override void OnShow()
    {
        m_Button.interactable = false;
        m_Text.text = "人妻海澜";
    }
}
