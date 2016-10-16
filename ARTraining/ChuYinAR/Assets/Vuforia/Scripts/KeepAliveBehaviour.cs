/*==============================================================================
Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// The KeepAliveBehaviour allows Vuforia objects to be reused across multiple
    /// scenes. This makes it possible to share datasets and targets between scenes.
    /// </summary>
    [RequireComponent(typeof (VuforiaBehaviour))]
    public class KeepAliveBehaviour : KeepAliveAbstractBehaviour
    {
    }
}
