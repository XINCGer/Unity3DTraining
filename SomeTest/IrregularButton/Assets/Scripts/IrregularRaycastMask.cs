using UnityEngine;
using UnityEngine.UI;

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

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        _sprite = _image.sprite;

        var rectTransform = (RectTransform)transform;
        Vector2 localPositionPivotRelative;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, sp, eventCamera, out localPositionPivotRelative);

        // convert to bottom-left origin coordinates
        var localPosition = new Vector2(localPositionPivotRelative.x + rectTransform.pivot.x * rectTransform.rect.width,
            localPositionPivotRelative.y + rectTransform.pivot.y * rectTransform.rect.height);

        var spriteRect = _sprite.textureRect;
        var maskRect = rectTransform.rect;

        var x = 0;
        var y = 0;
        // convert to texture space
        switch (_image.type)
        {

            case Image.Type.Sliced:
                {
                    var border = _sprite.border;
                    // x slicing
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
                    // y slicing
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
                    // conversion to uniform UV space
                    x = Mathf.FloorToInt(spriteRect.x + spriteRect.width * localPosition.x / maskRect.width);
                    y = Mathf.FloorToInt(spriteRect.y + spriteRect.height * localPosition.y / maskRect.height);
                }
                break;
        }

        // destroy component if texture import settings are wrong
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
