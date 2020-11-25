using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFog : MonoBehaviour
{
  public Rigidbody rb;
  public float startFog;
  public PlayerCollision pc;

  private void Start() {
    startFog = RenderSettings.fogDensity; 
  }

    // Update is called once per frame
  void FixedUpdate() {
    RenderSettings.fogDensity = (1f / (1f+(0.0002f*rb.velocity.z))) * RenderSettings.fogDensity;
    if (pc.hit && RenderSettings.fogDensity < startFog) {
      RenderSettings.fogDensity *= 1.05f;
    }
  }
}
