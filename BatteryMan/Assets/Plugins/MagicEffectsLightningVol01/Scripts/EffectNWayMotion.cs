// (c) Copyright 2013 Luke Light&Magic. All rights reserved.

using UnityEngine;
using System.Collections;

public class EffectNWayMotion : MonoBehaviour {
  public int DirectionWay = 8;
  public Transform WayChild;
  public bool UseSpreadDirection = false;
  public int SpreadAngle = 120;
  public float StopEffectsTime = 0;

  private GameObject effectRoot;

  void CreateWayMotion() {
    if (WayChild == null) {
      Debug.Log("WayChild is Null!");
      return;
    }

    if (StopEffectsTime >0) {
      StopCoroutine("StopEffects");
      StartCoroutine(StopEffects(StopEffectsTime));
    }

    if (effectRoot == null) {
      effectRoot = new GameObject();
      effectRoot.name = "effectRoot(clone)";
      effectRoot.transform.parent = transform;
    }
    float angle = 0;
    if (!UseSpreadDirection) {
      //Circle Direction
      WayChild.gameObject.SetActive(true);
      angle = 360 / DirectionWay;
      for (var i = 1; i < DirectionWay; i++) {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y + angle * i, 0));
        Transform effect = Instantiate(WayChild, transform.position, rotation) as Transform;
        effect.parent = effectRoot.transform;
      }
    }
    else {
      //Spread Direction
      WayChild.gameObject.SetActive(false);
      angle = SpreadAngle / DirectionWay;
      for (var i = 0; i < DirectionWay; i++) {
        float startAngle = 0;
        if (DirectionWay % 2 == 0)
          startAngle = angle / 2f;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, transform.localEulerAngles.y + startAngle + angle * (-DirectionWay / 2 + i), 0));
        Transform effect = Instantiate(WayChild, transform.position, rotation) as Transform;
        effect.parent = effectRoot.transform;
        effect.gameObject.SetActive(true);
      }
    }

	}


  void OnEnable() {
    CreateWayMotion();
  }

  void OnDisable() {
    if (effectRoot != null)
      Destroy(effectRoot);
  }

  IEnumerator StopEffects(float waitTime) {
    yield return new WaitForSeconds(waitTime);
    WayChild.gameObject.SetActive(false);
    effectRoot.gameObject.SetActive(false);
    OnDisable();
  }

}
