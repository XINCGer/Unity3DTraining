using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tool;

public class ChangeInspector : MonoBehaviour {

    private Texture texture;
    private Rect rectValue;
    private string remark;//备注
    private float sliderValue = 0.8f;//进度值
    private bool isOpen = true;
    private EDirType type=EDirType.up;
    private float theValue1 = 0f;
    private float theValue2 = 0f;
    private bool isAdd = false;

    public Texture Texture1 {
        get { return texture; }
        set { texture = value; }
    }

    public Rect RectValue {
        get { return rectValue; }
        set { rectValue = value; }
    }

    public string Remark {
        get { return remark; }
        set { remark = value; }
    }

    public float SliderValue {
        get { return sliderValue; }
        set { sliderValue = value; }
    }

    public bool IsOpen {
        get { return isOpen; }
        set { isOpen = value; }
    }

    public EDirType Type
    {
        get { return type; }
        set { type = value; }
    }

    public float TheValue1
    {
        get { return theValue1; }
        set { theValue1 = value; }
    }

    public float TheValue2
    {
        get { return theValue2; }
        set { theValue2 = value; }
    }

    public bool IsAdd
    {
        get { return isAdd; }
        set { isAdd = value; }
    }
}
