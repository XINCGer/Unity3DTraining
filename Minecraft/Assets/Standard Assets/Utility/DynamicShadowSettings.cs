using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class DynamicShadowSettings : MonoBehaviour
    {
        public Light sunLight;
        public float minHeight = 10;
        public float minShadowDistance = 80;
        public float minShadowBias = 1;
        public float maxHeight = 1000;
        public float maxShadowDistance = 10000;
        public float maxShadowBias = 0.1f;
        public float adaptTime = 1;

        private float m_SmoothHeight;
        private float m_ChangeSpeed;
        private float m_OriginalStrength = 1;


        private void Start()
        {
            m_OriginalStrength = sunLight.shadowStrength;
        }


        // Update is called once per frame
        private void Update()
        {
            Ray ray = new Ray(Camera.main.transform.position, -Vector3.up);
            RaycastHit hit;
            float height = transform.position.y;
            if (Physics.Raycast(ray, out hit))
            {
                height = hit.distance;
            }

            if (Mathf.Abs(height - m_SmoothHeight) > 1)
            {
                m_SmoothHeight = Mathf.SmoothDamp(m_SmoothHeight, height, ref m_ChangeSpeed, adaptTime);
            }

            float i = Mathf.InverseLerp(minHeight, maxHeight, m_SmoothHeight);

            QualitySettings.shadowDistance = Mathf.Lerp(minShadowDistance, maxShadowDistance, i);
            sunLight.shadowBias = Mathf.Lerp(minShadowBias, maxShadowBias, 1 - ((1 - i)*(1 - i)));
            sunLight.shadowStrength = Mathf.Lerp(m_OriginalStrength, 0, i);
        }
    }
}
