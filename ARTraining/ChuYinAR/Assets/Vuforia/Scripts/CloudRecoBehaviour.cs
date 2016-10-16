/*==============================================================================
Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// This is the main behaviour class that encapsulates cloud recognition behaviour.
    /// It just has to be added to a Vuforia-enabled Unity scene and will initialize the target finder and wait for new results.
    /// State changes and new results will be sent to registered ICloudRecoEventHandlers
    /// </summary> 
    public class CloudRecoBehaviour : CloudRecoAbstractBehaviour
    {
    }
}
