using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Laser : MonoBehaviour {

	public float m_delay = 0f;
	public float m_lifeTime = 1f;
	public bool m_isLoop = true;
	public float m_defaultLength = 5f;
	public float m_startSize = 1f;
	public float m_startHigh = 0f;
	public float m_endHigh = 0f;
	public Vector2 m_Title = new Vector2(1, 1);
	public Vector2 m_uvOffsetSpeed = new Vector2(0f, 0f);
	public Material m_material = null;
	static readonly Keyframe[] defaultKeyframe = new Keyframe[]{ new Keyframe(0, 1), new Keyframe(1, 1) };
	public AnimationCurve m_scaleCurve = new AnimationCurve(defaultKeyframe);
	public AnimationCurve m_widthCurve = new AnimationCurve(defaultKeyframe);
	public Gradient m_colorGradient = new Gradient();
	public Gradient m_lineColorGradient = new Gradient();
	public float m_hitRadius = 0f;
	public GameObject m_hitEffect = null;

	public Transform m_startTransfrom;
	public Transform m_endTransfrom;

	private LineRenderer m_lineRender;
	private float m_delayEndTime = 0;

	private float m_passTime = 0f;
	private Vector3 m_curStartPos = new Vector3();
	private Vector3 m_curEndPos = new Vector3();
	private float m_curSize = 1f;
	private Color m_curColor = Color.white;
	private Vector2 m_titleUVLength = new Vector2(1, 1);
	private Vector2 m_titleUVOffset = new Vector2(1, 1);
	private bool m_firstSetUV = true;
	private Vector2 m_curUVOffset = new Vector2(0f, 0f);
	// Use this for initialization
	void Start () {
		if (m_lineRender == null)
		{
			m_lineRender = GetComponent<LineRenderer>();
			if (m_lineRender == null)
			{
				m_lineRender = this.gameObject.AddComponent<LineRenderer>();
			}
		}

		m_lineRender.shadowCastingMode = ShadowCastingMode.Off;
		m_lineRender.receiveShadows = false;
		m_lineRender.textureMode = LineTextureMode.Tile;
		if (m_material != null)
		{
			m_lineRender.material = new Material(m_material);
		}

		m_lineRender.widthCurve = new AnimationCurve(m_widthCurve.keys);
		CaculateCurWidthCurve();

		Gradient newGradient = new Gradient();
		newGradient.mode = m_lineColorGradient.mode;
		newGradient.SetKeys(m_lineColorGradient.colorKeys, m_lineColorGradient.alphaKeys);
		m_lineRender.colorGradient = newGradient;
		//m_lineRender.colorGradient.SetKeys(m_lineColorGradient.colorKeys, m_lineColorGradient.alphaKeys);

		CaculateColorGradient();
	}

	private void OnEnable()
	{
		m_delayEndTime = Time.timeSinceLevelLoad + m_delay;

		if (m_Title.x < 1)
			m_Title.x = 1;
		if (m_Title.y < 1)
			m_Title.y = 1;

		if ((int)m_Title.x <= 1)
			m_titleUVOffset.x = 0;
		else
		{
			m_titleUVOffset.x = Random.Range(1, (int)m_Title.x + 1);
			m_titleUVOffset.x = (m_titleUVOffset.x - 1) / Mathf.Floor(m_Title.x);
		}
			
		if ((int)m_Title.y <= 1)
			m_titleUVOffset.y = 1;
		else
		{
			m_titleUVOffset.y = Random.Range(1, (int)m_Title.y + 1);
			m_titleUVOffset.y = (m_titleUVOffset.y - 1) / Mathf.Floor(m_Title.y);
		}


		m_titleUVLength.x = 1f / Mathf.Floor(m_Title.x);
		m_titleUVLength.y = 1f / Mathf.Floor(m_Title.y);

		m_firstSetUV = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(m_lifeTime <= 0 || m_delayEndTime > Time.timeSinceLevelLoad)
		{
			if (m_lineRender)
				m_lineRender.enabled = false;

			if (m_hitEffect)
				m_hitEffect.SetActive(false);
			return;
		}

		m_passTime = Time.timeSinceLevelLoad - m_delayEndTime;
		if(m_passTime > m_lifeTime)
		{
			if(m_isLoop)
			{
				m_passTime -= m_lifeTime * ((int)(m_passTime / m_lifeTime));
			}else
			{
				if (m_lineRender)
					m_lineRender.enabled = false;

				if (m_hitEffect)
					m_hitEffect.SetActive(false);

				return;
			}
		}

		if (m_lineRender)
			m_lineRender.enabled = true;
		if (m_hitEffect)
			m_hitEffect.SetActive(true);

		if (m_startTransfrom == null)
		{
			m_curStartPos = this.transform.position;
		}else
		{
			m_curStartPos = m_startTransfrom.position;
			m_curStartPos.y += m_startHigh;
		}

		if(m_endTransfrom == null)
		{
			m_curEndPos = this.transform.rotation * Vector3.forward * m_defaultLength + m_curStartPos; 
		}else
		{
			m_curEndPos = m_endTransfrom.position;
			m_curEndPos.y += m_endHigh;

			if(m_hitRadius > 0 )
			{
				float orginLength = (m_curEndPos - m_curStartPos).magnitude;
				float lineLength = orginLength - m_hitRadius;
				if (lineLength < 0 )
				{
					m_curEndPos = m_curStartPos;
				}else
				{
					m_curEndPos = Vector3.Lerp(m_curStartPos, m_curEndPos, lineLength / orginLength);
				}
			}
		}
			
		m_lineRender.positionCount = 2;
		m_lineRender.SetPosition(0, m_curStartPos);
		m_lineRender.SetPosition(1, m_curEndPos);

		CaculateColorGradient();
		CaculateCurWidthCurve();
		CaculateUVOffset();

		if(m_hitEffect != null)
		{
			m_hitEffect.transform.position = m_curEndPos;
			if((m_curStartPos - m_curEndPos).magnitude > 0.001f)
			{
				m_hitEffect.transform.rotation = Quaternion.LookRotation(m_curEndPos - m_curStartPos);
			}
		}

		//if (Mathf.Abs(m_uvOffsetSpeed.x)> 0.0001f || Mathf.Abs(m_uvOffsetSpeed.y) > 0.0001f)
		//{
		//	float offsetX = (Time.timeSinceLevelLoad - m_delayEndTime) * m_uvOffsetSpeed.x;
		//	if(offsetX > 1f)
		//	{
		//		offsetX -= (int)offsetX;
		//	}else if(offsetX < -1f)
		//	{
		//		offsetX += (1 + (int)Mathf.Abs(offsetX));
		//	}

		//	m_material.SetTextureOffset("_MainTex", new Vector2(offsetX, 0));
		//}
	}

	public void CaculateCurWidthCurve()
	{
		if (m_lineRender == null)
			return;

		float curScale = 1f;
		if (m_scaleCurve.length >= 2)
		{
			curScale = m_scaleCurve.Evaluate(m_passTime / m_lifeTime);
		}

		int framecount = m_lineRender.widthCurve.length;
		AnimationCurve curCurve = new AnimationCurve();
		for (int i = 0; i < framecount; i++)
		{
			float time = m_lineRender.widthCurve.keys[i].time;
			float value = curScale * m_startSize * m_widthCurve.Evaluate(time);
			//m_lineRender.widthCurve.keys[i] = new Keyframe(time, value);
			curCurve.AddKey(new Keyframe(time, value));
		}
		m_lineRender.widthCurve = curCurve;

		//m_lineRender.startWidth = m_startSize * curScale * m_withCurve.Evaluate(0);
		//m_lineRender.endWidth = m_startSize * curScale * m_withCurve.Evaluate(1);
	}

	public void CaculateColorGradient()
	{
		if (m_lineRender == null)
			return;

		Gradient newGradient = new Gradient();
		newGradient.mode = m_lineColorGradient.mode;
		//

		m_curColor = m_colorGradient.Evaluate(m_passTime / m_lifeTime);
		int count = m_lineRender.colorGradient.colorKeys.Length;
		GradientColorKey[] newColorKeys = new GradientColorKey[count];
		for (int i = 0; i < count; i++)
		{
			float time = m_lineRender.colorGradient.colorKeys[i].time;
			Color newColor = m_curColor * m_lineColorGradient.Evaluate(m_lineRender.colorGradient.colorKeys[i].time);
			newColorKeys[i] = new GradientColorKey(newColor, time);
		}

		count = m_lineRender.colorGradient.alphaKeys.Length;
		GradientAlphaKey[] newAlphaKeys = new GradientAlphaKey[count];
		for (int i = 0; i < count; i++)
		{
			float time = m_lineRender.colorGradient.alphaKeys[i].time;
			Color newColor = m_curColor * m_lineColorGradient.Evaluate(m_lineRender.colorGradient.alphaKeys[i].time);
			newAlphaKeys[i] = new GradientAlphaKey(newColor.a, time);
			//curGradient.
		}
		newGradient.SetKeys(newColorKeys, newAlphaKeys);
		m_lineRender.colorGradient = newGradient;
		//m_lineRender.colorGradient.SetKeys(newColorKeys, newAlphaKeys);
	}

	public void CaculateUVOffset()
	{
		if (m_lineRender == null)
			return;

		if (Mathf.Abs(m_uvOffsetSpeed.x) < 0.0001f && Mathf.Abs(m_uvOffsetSpeed.y) < 0.0001f && m_firstSetUV == false)
			return;

		if(m_firstSetUV)
		{
			new Texture();
			m_lineRender.material.SetTextureScale("_MainTex", m_titleUVLength);
			m_firstSetUV = false;
		}
		
		float offsetX = (Time.timeSinceLevelLoad - m_delayEndTime) * m_uvOffsetSpeed.x;
		float count = Mathf.Floor(offsetX / m_titleUVLength.x);

		if(offsetX >= 0)
		{
			m_curUVOffset.x = m_titleUVOffset.x + (offsetX - count * m_titleUVLength.x);
		} else
		{
			m_curUVOffset.x = m_titleUVOffset.x + m_titleUVLength.x + (offsetX + count * m_titleUVLength.x);
		}

		float offsetY = (Time.timeSinceLevelLoad - m_delayEndTime) * m_uvOffsetSpeed.y;
		count = Mathf.Floor(offsetY / m_titleUVLength.y);

		if (offsetY >= 0)
		{
			m_curUVOffset.y = m_titleUVOffset.y + (offsetY - count * m_titleUVLength.y);
		} else
		{
			m_curUVOffset.y = m_titleUVOffset.y + m_titleUVLength.y + (offsetY + count * m_titleUVLength.y);
		}

		m_lineRender.material.SetTextureOffset("_MainTex", m_curUVOffset);
	}
}
