// (c) Copyright 2013 Luke Light&Magic. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectLightsFade : MonoBehaviour {
  public float totalTime;
  public float fadeInTime;
  public float fadeOutTime;

  private Light dynamicLight;

  void Awake() {
    dynamicLight = GetComponent<Light>();
    if (dynamicLight != null) {
      float intensity = dynamicLight.intensity;
      Animation animLight = GetComponent<Animation>();
      if (animLight == null)
        gameObject.AddComponent<Animation>();
      else
        return;
      if (fadeInTime < 0) fadeInTime = 0;
      if (fadeInTime > totalTime) fadeInTime = totalTime;
      if (fadeOutTime < 0) fadeOutTime = 0;
      if (fadeOutTime > totalTime) fadeOutTime = totalTime;
      if (fadeInTime + fadeOutTime > totalTime) fadeInTime = 0;

      List<Keyframe> keyList = new List<Keyframe>();
      if (fadeInTime != 0) {
        AnimationCurve linearCurve = AnimationCurve.Linear(0, 0, fadeInTime, intensity);
        keyList.Add(linearCurve[0]);
        keyList.Add(linearCurve[1]);
      }
      if (fadeOutTime != 0) {
        AnimationCurve linearCurve = AnimationCurve.Linear(totalTime - fadeOutTime, intensity, totalTime, 0);
        if (fadeInTime == 0 || fadeInTime != totalTime - fadeOutTime)
          keyList.Add(linearCurve[0]);
        else {
          Keyframe key = keyList[1];
          key.outTangent = linearCurve[0].outTangent;
          keyList[1] = key;
        }
        keyList.Add(linearCurve[1]);
      }
      AnimationCurve curve = new AnimationCurve(keyList.ToArray());
      AnimationClip clip = new AnimationClip();
      clip.SetCurve("", typeof(Light), "m_Intensity", curve);
     // animation.clip = clip;
      GetComponent<Animation>().clip = clip;
      GetComponent<Animation>().name = "light_fade";
      //animation.clip.name = "light_fade";
      gameObject.SetActive(false);
      gameObject.SetActive(true);
    }

	}


  void Reset() {
    totalTime = 1;
    fadeInTime = 0;
    fadeOutTime = 1;
  }
}
