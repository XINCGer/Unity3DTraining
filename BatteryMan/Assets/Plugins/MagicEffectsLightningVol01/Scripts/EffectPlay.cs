// (c) Copyright 2013 Luke Light&Magic. All rights reserved.

using UnityEngine;
using System.Collections;

public class EffectPlay : MonoBehaviour {
  public Transform surround_point, ground;
  public Transform[] effects = { null };

  private int effectIndex, goIndex, fpsNumber;
  private float repeatTime = 4;
  private float timer;
  private float fpsTimer;
  private Transform effect;
  private float fps, fpsTotal;
  private bool isRepeat = true;
  private string effectName;


  void Start() {
    InitEffect();
  }

  void OnGUI() {
    GUILayout.Label("Effect Name : " + effectName.Replace("(Clone)",""));
    GUILayout.Label("Effect Index : " + effectIndex);
    GUILayout.Label("Total Effects : " + effects.Length);

    GUILayout.BeginHorizontal();
    if (GUILayout.Button("Go Effect Index", GUILayout.Width(120))) {
      if (goIndex == effectIndex || goIndex < 0 || goIndex >= effects.Length)
        return;
      if (effect != null)
        Destroy(effect.gameObject);
      effectIndex = goIndex;
      InitEffect();
    }
    GUILayout.Label(" ==> ", GUILayout.Width(30));
    goIndex = int.Parse(GUILayout.TextField(goIndex.ToString(), GUILayout.MaxWidth(50)));
    GUILayout.EndHorizontal();
    
    GUILayout.BeginHorizontal();
    GUILayout.Label("Repeat Time : ", GUILayout.MaxWidth(85));
    repeatTime = float.Parse(GUILayout.TextField(repeatTime.ToString(), GUILayout.MaxWidth(50)));
    GUILayout.EndHorizontal();

    if (GUILayout.Button("Show/Hide Ground", GUILayout.Width(120))) {
      if (ground != null) {
        ground.gameObject.SetActive(!ground.gameObject.activeSelf);
      }
    }
    GUILayout.Space(6);

    if (GUILayout.Button("Previous Effect", GUILayout.Width(120))) {
      if (effect != null)
        Destroy(effect.gameObject);
      --effectIndex;
      if (effectIndex <0)
        effectIndex = effects.Length - 1;
      InitEffect();
    }
    else if (GUILayout.Button("Next Effect", GUILayout.Width(120))) {
      if (effect != null)
        Destroy(effect.gameObject);
      ++effectIndex;
      if (effectIndex >= effects.Length)
        effectIndex = 0;
      InitEffect();
    }
    GUILayout.Space(6);
    GUILayout.Label("fps = " + fps);
  }

  void Update() {
    if (timer >= repeatTime && isRepeat) {
      if (effect != null) {
        effect.gameObject.SetActive(false);
        StartCoroutine(DelayActive(effect.gameObject));
      }
      timer = 0;
    }
    else
      timer += Time.deltaTime;

    if (Time.time > fpsTimer) {
      fps = fpsTotal / fpsNumber;
      fpsTimer = Time.time + 0.5f;
    }
    else {
      fpsTotal += 1 / Time.deltaTime;
      ++fpsNumber;
    }
  }

  void InitEffect() {
    effect = null;
    effect = effects[effectIndex];
    if (effect != null)
      effect = Instantiate(effect) as Transform;
    if (effectIndex >= 6 && effectIndex <= 21) {
      if (surround_point != null) {
        effect.parent = surround_point;
        if (effectIndex >= 6 && effectIndex <= 9)
          effect.localPosition = new Vector3(0, effect.localPosition.y, 0);
        else
          effect.localPosition = Vector3.zero;
        isRepeat = false;
      }
    }
    else if (effectIndex >= 22 && effectIndex <= 24)
      isRepeat = false;
    else
      isRepeat = true;
    if (effectIndex >= 29 && effectIndex <= 32)
      effect.localEulerAngles = new Vector3(0, -45, 0);
    timer = 0;
    effectName = effect.name;
  }

  IEnumerator DelayActive(GameObject effect) {
    yield return new WaitForSeconds(0.1f);
    if (effect != null)
      effect.SetActive(true);
  }

}
