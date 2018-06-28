using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 不规则区域Button
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
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
