/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.IO;
using UnityEditor;

namespace Vuforia.EditorClasses
{
    /// <summary>
    /// Small utility class to create an instance of the VuforiaBehaviourComponentFactory in the editor before anything is initialized.
    /// </summary>
    [InitializeOnLoad]
    public class ComponentFactoryStarter
    {
        /// <summary>
        /// register an instance of the VuforiaBehaviourComponentFactory class at the singleton immediately
        /// </summary>
        static ComponentFactoryStarter()
        {
            BehaviourComponentFactory.Instance = new VuforiaBehaviourComponentFactory();
        }
    }
}
