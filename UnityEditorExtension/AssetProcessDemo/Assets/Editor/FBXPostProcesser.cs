using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class FBXPostProcesser : AssetPostprocessor
{
    #region 模型处理
    /// <summary>
    /// 模型导入之前调用  
    /// </summary>
    public void OnPreprocessModel()
    {

        //判断资源是否是首次导入
#if UNITY_2019_3_OR_NEWER
        if (!assetImporter.importSettingsMissing)
        {
            assetImporter.userData = "v_0.0.1";
            return;
        }
#else
        var metaPath = this.assetPath + ".meta";
        if (File.Exists(metaPath))
        {
            return;
        }
#endif

        Debug.Log("==模型导入之前调用==" + this.assetPath);
        ModelImporter modelImporter = (ModelImporter)assetImporter;

        //模型优化
        modelImporter.optimizeMesh = true;
        modelImporter.optimizeGameObjects = true;
        modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;
        modelImporter.animationRotationError = 1.0f;
        modelImporter.animationPositionError = 1.0f;
        modelImporter.animationScaleError = 1.0f;
    }


    /// <summary>
    /// 模型导入之后调用  
    /// </summary>
    /// <param name="go"></param>
    public void OnPostprocessModel(GameObject go)
    {
        //判断资源是否是首次导入
#if UNITY_2019_3_OR_NEWER
        if (!assetImporter.importSettingsMissing)
        {
            return;
        }
#else
        var metaPath = this.assetPath + ".meta";
        if (File.Exists(metaPath))
        {
            return;
        }
#endif

        // for skeleton animations.
        Debug.Log("==模型导入之后调用==");
        List<AnimationClip> animationClipList = new List<AnimationClip>(AnimationUtility.GetAnimationClips(go));
        if (animationClipList.Count == 0)
        {
            AnimationClip[] objectList = Object.FindObjectsOfType(typeof(AnimationClip)) as AnimationClip[];
            animationClipList.AddRange(objectList);
        }

        foreach (AnimationClip theAnimation in animationClipList)
        {
            try
            {
                //  去除scale曲线
                //foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(theAnimation))
                //{
                //    string name = theCurveBinding.propertyName.ToLower();
                //    if (name.Contains("scale"))
                //    {
                //        AnimationUtility.SetEditorCurve(theAnimation, theCurveBinding, null);
                //    }
                //}

                // 浮点数精度压缩到f3
                AnimationClipCurveData[] curves = null;
                curves = AnimationUtility.GetAllCurves(theAnimation);
                Keyframe key;
                Keyframe[] keyFrames;
                for (int ii = 0; ii < curves.Length; ++ii)
                {
                    AnimationClipCurveData curveDate = curves[ii];
                    if (curveDate.curve == null || curveDate.curve.keys == null)
                    {
                        //Debuger.LogWarning(string.Format("AnimationClipCurveData {0} don't have curve; Animation name {1} ", curveDate, animationPath));
                        continue;
                    }
                    keyFrames = curveDate.curve.keys;
                    for (int i = 0; i < keyFrames.Length; i++)
                    {
                        key = keyFrames[i];
                        key.value = float.Parse(key.value.ToString("f3"));
                        key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                        key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                        keyFrames[i] = key;
                    }
                    curveDate.curve.keys = keyFrames;
                    theAnimation.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("CompressAnimationClip Failed !!! animationPath : {0} error: {1}", assetPath, e));
            }
        }
    }
    #endregion

}
