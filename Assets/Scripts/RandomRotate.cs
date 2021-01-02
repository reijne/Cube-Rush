using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
  public Rigidbody rb;
  private float speed = 0.5f;
  
  // Update is called once per frame
  void FixedUpdate()
  {
    if (TitleScreen.loading) speed = 3;
    rb.AddTorque(new Vector3(Random.Range(-speed, speed), Random.Range(-speed, speed), Random.Range(-speed, speed)), ForceMode.Force);
  }
}
