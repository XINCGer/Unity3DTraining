/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// Helper behaviour used to hide augmented objects behind the video background.
    /// </summary>
    public class MaskOutBehaviour : MaskOutAbstractBehaviour
    {
        #region UNITY_MONOBEHAVIOUR_METHODS

        void Start ()
        {
            if (VuforiaRuntimeUtilities.IsVuforiaEnabled())
            {
                Renderer rendererComp = GetComponent<Renderer>();
                int numMaterials = rendererComp.materials.Length;
                if (numMaterials == 1)
                {
                    rendererComp.sharedMaterial = maskMaterial;
                }
                else
                {
                    Material[] maskMaterials = new Material[numMaterials];
                    for (int i = 0; i < numMaterials; i++)
                        maskMaterials[i] = maskMaterial;

                    rendererComp.sharedMaterials = maskMaterials;
                }
            }
        }

        #endregion // UNITY_MONOBEHAVIOUR_METHODS
    }
}
