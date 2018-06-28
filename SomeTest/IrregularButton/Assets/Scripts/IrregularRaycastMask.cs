using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 不规则区域图形的射线检测Mask组件
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class IrregularRaycastMask : MonoBehaviour, ICanvasRaycastFilter
{
    private Image _image;
    private Sprite _sprite;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// 重写IsRaycastLocationValid接口
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        _sprite = _image.sprite;

        var rectTransform = (RectTransform)transform;
        Vector2 localPositionPivotRelative;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out localPositionPivotRelative);

        // 转换为以屏幕左下角为原点的坐标系
        var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
            localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);

        var spriteRect = _sprite.textureRect;
        var maskRect = rectTransform.rect;

        var x = 0;
        var y = 0;
        // 转换为纹理空间坐标
        switch (_image.type)
        {

            case Image.Type.Sliced:
                {
                    var border = _sprite.border;
                    // x 轴裁剪
                    if (localPosition.x < border.x)
                    {
                        x = Mathf.FloorToInt(spriteRect.x + localPosition.x);
                    }
                    else if (localPosition.x > maskRect.width - border.z)
                    {
                        x = Mathf.FloorToInt(spriteRect.x + spriteRect.width - (maskRect.width - localPosition.x));
                    }
                    else
                    {
                        x = Mathf.FloorToInt(spriteRect.x + border.x +
                                             ((localPosition.x - border.x) /
                                             (maskRect.width - border.x - border.z)) *
                                             (spriteRect.width - border.x - border.z));
                    }
                    // y 轴裁剪
                    if (localPosition.y < border.y)
                    {
                        y = Mathf.FloorToInt(spriteRect.y + localPosition.y);
                    }
                    else if (localPosition.y > maskRect.height - border.w)
                    {
                        y = Mathf.FloorToInt(spriteRect.y + spriteRect.height - (maskRect.height - localPosition.y));
                    }
                    else
                    {
                        y = Mathf.FloorToInt(spriteRect.y + border.y +
                                             ((localPosition.y - border.y) /
                                             (maskRect.height - border.y - border.w)) *
                                             (spriteRect.height - border.y - border.w));
                    }
                }
                break;
            case Image.Type.Simple:
            default:
                {
                    // 转换为统一UV空间
                    x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
                    y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
                }
                break;
        }

        // 如果texture导入过程报错，则删除组件
        try
        {
            return _sprite.texture.GetPixel(x, y).a > 0;
        }
        catch (UnityException e)
        {
            Debug.LogError("Mask texture not readable, set your sprite to Texture Type 'Advanced' and check 'Read/Write Enabled'" + e.Message);
            Destroy(this);
            return false;
        }
    }
}
