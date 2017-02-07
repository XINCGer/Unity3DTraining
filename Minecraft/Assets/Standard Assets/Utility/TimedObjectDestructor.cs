using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class TimedObjectDestructor : MonoBehaviour
    {
        [SerializeField] private float m_TimeOut = 1.0f;
        [SerializeField] private bool m_DetachChildren = false;


        private void Awake()
        {
            Invoke("DestroyNow", m_TimeOut);
        }


        private void DestroyNow()
        {
            if (m_DetachChildren)
            {
                transform.DetachChildren();
            }
            DestroyObject(gameObject);
        }
    }
}
