/*===============================================================================
Copyright (c) 2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2013-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
  
Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// This script renders the mesh from the MeshFilter as wireframe.
    /// This is mainly supposed to be used for visualization/debugging purpoes. It uses GL.LINES to draw the wireframe,
    /// which is not very fast for larger meshes.
    /// In order to draw large wireframe meshes in an app, it is recommended to use 3rd party libraries such as Vectrosity.
    /// </summary>
    public class WireframeBehaviour : MonoBehaviour
    {
        #region PUBLIC_MEMBERS
        
        public Material lineMaterial;
        public bool ShowLines = true;
        public Color LineColor = Color.green;
        
        #endregion // PUBLIC_MEMBERS

        
        #region PRIVATE_MEMBERS
        
        private Material mLineMaterial;
        
        #endregion // PRIVATE_MEMBERS


        #region UNITY_MONOBEHAVIOUR_METHODS
        
        void Start()
        {
            if (lineMaterial != null) 
            {
                // We clone the material so to have a unique instance
                // for each WireframeBehaviour instance
                mLineMaterial = new Material(lineMaterial);
            } 
            else 
            {
                Debug.LogWarning ("Missing line material for wireframe rendering!");
            }
        }

        void OnRenderObject ()
        {
            // avoid lines being rendered in Background-camera
            GameObject go = VuforiaManager.Instance.ARCameraTransform.gameObject;
            Camera[] cameras = go.GetComponentsInChildren<Camera>();
            bool valid = false;
            foreach (Camera cam in cameras)
            {
                if(Camera.current == cam)
                    valid = true;
            }
            if(!valid)
                return;
        
            if (!ShowLines) return;

            var mf = GetComponent<MeshFilter>();
            if (!mf) return;


            if (mLineMaterial == null) 
            {
                Debug.LogWarning ("Missing line material for wireframe rendering!");
                return;
            }

            var mesh = mf.sharedMesh;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;
    
            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);

            mLineMaterial.SetPass(0);
            mLineMaterial.SetColor ("_Color", LineColor);

            GL.Begin(GL.LINES); 
            for (int i=0; i<triangles.Length; i+=3) {

                var P0 = (vertices[triangles[i+0]]);
                var P1 = (vertices[triangles[i+1]]);
                var P2 = (vertices[triangles[i+2]]);
            
                GL.Vertex(P0);
                GL.Vertex(P1);
                GL.Vertex(P1);
                GL.Vertex(P2);
                GL.Vertex(P2);
                GL.Vertex(P0);
            }
    
            GL.End();
            GL.PopMatrix();
        }

        void OnDrawGizmos()
        {
            if (ShowLines && enabled)
            {

                var mf = GetComponent<MeshFilter>();
                if (!mf) return;

                Gizmos.matrix = Matrix4x4.TRS(gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.lossyScale);
                Gizmos.color = LineColor;

                var mesh = mf.sharedMesh;
                if (mesh != null)
                {
                    var vertices = mesh.vertices;
                    var triangles = mesh.triangles;
                    for (int i = 0; i < triangles.Length; i += 3)
                    {
                        var P0 = (vertices[triangles[i + 0]]);
                        var P1 = (vertices[triangles[i + 1]]);
                        var P2 = (vertices[triangles[i + 2]]);

                        Gizmos.DrawLine(P0, P1);
                        Gizmos.DrawLine(P1, P2);
                        Gizmos.DrawLine(P2, P0);
                    }
                }
            }
        }

        #endregion // UNITY_MONOBEHAVIOUR_METHODS
    }
}
