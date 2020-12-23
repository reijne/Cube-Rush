using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotate : MonoBehaviour
{
  public Rigidbody rb;
  
  // Update is called once per frame
  void FixedUpdate()
  {
    rb.AddTorque(new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), ForceMode.Force);
  }
}
