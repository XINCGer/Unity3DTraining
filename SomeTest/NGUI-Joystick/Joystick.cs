using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum SkillType
{
    Direction,
    Location
}

public class Joystick : MonoBehaviour
{    
    #region Delegate & Event
    public delegate void JoystickEventHandler(Joystick joystick);
    // 摇杆开始摇晃的回调
    public static event JoystickEventHandler On_JoystickMoveStart;
    //thumb偏离中心位置，并牌按住时，每帧的回调
    public static event JoystickEventHandler On_JoystickHolding;
    //摇杆按下
    public static event JoystickEventHandler On_JoystickDowm;
    // 松开摇杆的回调
    public static event JoystickEventHandler On_JoystickMoveEnd;

    public SkillType type;
    #endregion    

    #region   property

    private string m_joystickName = "NguiJoystick";
    public string JoystickName { get { return this.m_joystickName; } }
    private bool m_isLimitInCircle = true;
    public bool IsLimitInCircle { get { return this.m_isLimitInCircle; } }

   
    //joystickAxis  摇杆位置转为换 -1 到 1 之间的值
    private Vector2 m_joystickAxis = Vector2.zero;
    public Vector2 JoystickAxis{get { return this.m_joystickAxis; } }

    private Vector2 m_lastJoystickAxis = Vector2.zero;
    public Vector2 LastJoystickAxis { get { return this.m_lastJoystickAxis; } }
  
    // 判断joystick是否被禁用
    bool m_isForBid = false;
    public bool IsForBid { get { return this.m_isForBid; } }
    bool m_isHolding = false;
    public bool IsHolding { get { return this.m_isHolding; } }
    #endregion

    private UIWidget root;
	public UISprite bg;
	public UISprite thumb;
    public int radius = 60;

    void Awake()
    {
        this.name = this.JoystickName;
        root = this.GetComponent<UIWidget>();

        Init();
    }

	void Update ()
    {
        if (!m_isForBid && m_isHolding)
        {
            if (On_JoystickHolding != null)
            {
                On_JoystickHolding(this);
            }
        }

      
	}

    void Init()
    {
        bg.transform.localPosition = Vector3.zero;
        thumb.transform.localPosition = Vector3.zero;
       // SetJoystickSize(radius);
    }

	#region ngui event
    void OnPress (bool isPressed)
    {
        if (m_isForBid)
        {
            Debug.Log("joystick is forbid!");
            return;
        }
        Debug.Log("OnPress:" + isPressed.ToString());
        if(isPressed)
        {
            CalculateJoystickAxis();
            if(On_JoystickDowm !=null)
            {
                On_JoystickDowm(this);
            }
//            if (On_JoystickMoveStart != null)
//            {
//                On_JoystickMoveStart(this);
//            }
            m_isHolding = true;
        }
        else
        {
            CalculateJoystickAxis();
			if (On_JoystickMoveEnd != null)
            {
				On_JoystickMoveEnd(this);
            }
            thumb.transform.localPosition = Vector3.zero;
            m_isHolding = false;
        }
    }

    void OnDragStart ()
    {
        if (m_isForBid)
        {
            Debug.Log("joystick is forbid!");
            return;
        }

        Debug.Log("OnDragStart");
        CalculateJoystickAxis();
        if(On_JoystickMoveStart!=null)
        {
            On_JoystickMoveStart(this);
        }
	    m_isHolding = true;
       // Debug.Log(string.Format("time:{0} - axis:{1}", Time.time, joystickAxis));
    }

    void OnDrag(Vector2 delta)
    {
        if (m_isForBid)
        {
            return;
        }

//        Debug.Log("OnDrag:"+delta.ToString());        
        CalculateJoystickAxis();
//        if (On_JoystickMoveStart != null)
//        {
//            On_JoystickMoveStart(this);
//        }
    }


    private float CalculAngle(float x, float y)
    {
        float currX = x * 90 + 90;
        float currY = y * 90 + 90;
        if (currY < 90)
        {
            return 270 - currX;
        }
        else
        {
            if (currX < 90)
            {
                return currX + 270;
            }
            else
            {
                return  currX - 90;
            }
        }
    }

    void OnDragEnd ()
    {
        if (m_isForBid)
        {
            return;
        }

        Debug.Log("OnDragEnd");             
        CalculateJoystickAxis();
        //if (On_JoystickMoveEnd != null)
        //{
        //    On_JoystickMoveEnd(this);
        //}
        thumb.transform.localPosition = Vector3.zero;
        m_isHolding = false;
    }
	#endregion

    #region utile

    /// <summary>
    /// 计算JoystickAxis
    /// </summary>
    /// <returns></returns>
    void CalculateJoystickAxis()
    {
        Vector3 offset = ScreenPos_to_NGUIPos(UICamera.currentTouch.pos);
        offset -= transform.localPosition;
        if (m_isLimitInCircle)
        {
            if (offset.magnitude > radius)
            {
                offset = offset.normalized * radius;
            }
        }
        thumb.transform.localPosition = offset;

        m_lastJoystickAxis = m_joystickAxis;
        m_joystickAxis = new Vector2(offset.x / radius, offset.y / radius);
    }

    /// <summary>
    /// Axis2s the angle.
    /// </summary>
    /// <returns>
    /// The angle.
    /// </returns>
    public float Axis2Angle(bool inDegree = true)
    {
        float angle = Mathf.Atan2(m_joystickAxis.x, m_joystickAxis.y);

        if (inDegree)
        {
            return angle * Mathf.Rad2Deg;
        }
        else
        {
            return angle;
        }
    }

    /// <summary>
    /// Axis2s the angle.
    /// </summary>
    /// <returns>
    /// The angle.
    /// </returns>
    public float Axis2Angle(Vector2 axis, bool inDegree = true)
    {
        float angle = Mathf.Atan2(axis.x, axis.y);

        if (inDegree)
        {
            return angle * Mathf.Rad2Deg;
        }
        else
        {
            return angle;
        }
    }



    /// <summary>
    /// 屏幕坐标-->ui坐标
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    Vector3 ScreenPos_to_NGUIPos(Vector3 screenPos)
    {
        Vector3 uiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
        uiPos = UICamera.currentCamera.transform.InverseTransformPoint(uiPos);
        return uiPos;
    }

    /// <summary>
    /// 屏幕坐标-->ngui坐标
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns></returns>
    Vector3 ScreenPos_to_NGUIPos(Vector2 screenPos)
    {
        return ScreenPos_to_NGUIPos(new Vector3(screenPos.x, screenPos.y, 0f));
    }

    /// <summary>
    /// 设置摇杆的大小
    /// </summary>
    /// <param name="radius"></param>
    void SetJoystickSize(int radius)
    {
        root.width = 2 * radius;
        root.height = 2 * radius;
        thumb.width = (int)(30f / 100f * root.width);
        thumb.height = (int)(30f / 100f * root.height);
    }
    #endregion

    #region 激活、禁用的控制
    List<string> keys = new List<string>();
    
   /// <summary>
    /// 禁用
   /// </summary>
   /// <returns>返回值是，取消这个禁用要用到的key</returns>
    public string ForbidJosystick()
    {
        string key = System.Guid.NewGuid().ToString();
        keys.Add(key);
        m_isForBid = true;
        return key;
    }

    /// <summary>
    /// 启用
    /// </summary>
    /// <param name="key"></param>
    public void ActivizeJosystick(string key)
    {
        if(keys.Contains(key))
        {
            keys.Remove(key);
        }

        m_isForBid = true;
        if(keys.Count==0)
        {
            m_isForBid = false;
        }
    }

    #endregion
}
