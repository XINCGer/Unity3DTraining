/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// Small utility behaviour to create an instance of the VuforiaBehaviourComponentFactory at runtime before anything is initialized.
    /// </summary>
    public partial class ComponentFactoryStarterBehaviour : MonoBehaviour
    {
        /// <summary>
        /// call all member methods that have the FactoryStart attribute
        /// </summary>
        void Awake()
        {
            List<MethodInfo> methods = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();
            methods.AddRange(this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            foreach (MethodInfo methodInfo in methods)
            {
                foreach (Attribute attribute in methodInfo.GetCustomAttributes(true))
                {
                    if (attribute is FactorySetter)
                    {
                        #if NETFX_CORE   
                        Action factorySetMethod = methodInfo.CreateDelegate(typeof(Action), this) as Action;
                        #else
                        Action factorySetMethod = Delegate.CreateDelegate(typeof(Action), this, methodInfo) as Action;
                        #endif // NETFX_CORE
                        if (factorySetMethod != null)
                        {
                            factorySetMethod();
                        }
                    }
                }
            }
        }

        [FactorySetter]
        void SetBehaviourComponentFactory()
        {
            Debug.Log("Setting BehaviourComponentFactory");
            BehaviourComponentFactory.Instance = new VuforiaBehaviourComponentFactory();
        }
    }
}
