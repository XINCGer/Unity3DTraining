/*==============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.

Copyright (c) 2013-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// Factory class that adds child class Behaviours
    /// </summary>
    public class VuforiaBehaviourComponentFactory : IBehaviourComponentFactory
    {
        #region PUBLIC_METHODS

        public MaskOutAbstractBehaviour AddMaskOutBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<MaskOutBehaviour>();
        }

        public VirtualButtonAbstractBehaviour AddVirtualButtonBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<VirtualButtonBehaviour>();
        }

        public TurnOffAbstractBehaviour AddTurnOffBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<TurnOffBehaviour>();
        }

        public ImageTargetAbstractBehaviour AddImageTargetBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<ImageTargetBehaviour>();
        }

        public MarkerAbstractBehaviour AddMarkerBehaviour(GameObject gameObject)
        {
#pragma warning disable 618
            return gameObject.AddComponent<MarkerBehaviour>();
#pragma warning restore 618
        }

        public MultiTargetAbstractBehaviour AddMultiTargetBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<MultiTargetBehaviour>();
        }

        public CylinderTargetAbstractBehaviour AddCylinderTargetBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<CylinderTargetBehaviour>();
        }

        public WordAbstractBehaviour AddWordBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<WordBehaviour>();
        }

        public TextRecoAbstractBehaviour AddTextRecoBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<TextRecoBehaviour>();
        }

        public ObjectTargetAbstractBehaviour AddObjectTargetBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<ObjectTargetBehaviour>();
        }

        public VuMarkAbstractBehaviour AddVuMarkBehaviour(GameObject gameObject)
        {
            return gameObject.AddComponent<VuMarkBehaviour>();
        }

        #endregion // PUBLIC_METHODS
    }
}
