using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 不规则Button
/// </summary>
public class IrregulaButton : MonoBehaviour
{

    private void Awake()
    {
        var image = this.GetComponent<Image>();
        if (null != image)
        {
            image.alphaHitTestMinimumThreshold = 0.1f;
        }
    }
}
