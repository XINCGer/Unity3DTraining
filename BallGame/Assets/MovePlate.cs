using UnityEngine;
using System.Collections;

public class MovePlate : MonoBehaviour
{
  public float speed = 5.0f;

  void Update ()
  {
    float h = Input.GetAxis("Horizontal");
    transform.Translate(Vector3.right * h * speed * Time.deltaTime);
  }
}
