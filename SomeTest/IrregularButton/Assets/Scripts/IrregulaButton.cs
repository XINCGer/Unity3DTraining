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
    [Tooltip("设定Sprite响应的Alpha阈值")]
    [Range(0, 0.5f)]
    public float alpahThreshold = 0.5f;

    private void Awake()
    {
        var image = this.GetComponent<Image>();
        if (null != image)
        {
            image.alphaHitTestMinimumThreshold = alpahThreshold;
        }
    }
}
