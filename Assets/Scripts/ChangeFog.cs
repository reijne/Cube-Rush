using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFog : MonoBehaviour
{
  public Rigidbody rb;
  public float startFog;
  public PlayerCollision pc;
  public PlayerMovement pm;

  private void Start() {
    startFog = RenderSettings.fogDensity; 
  }

    // Update is called once per frame
  void FixedUpdate() {
    if (pm.infinite) {
      if (pm.speed > 0) {
        RenderSettings.fogDensity = (1f / (1f+(0.05f*(pm.speed+0.1f)))) * startFog;
      }
    } else {
      RenderSettings.fogDensity = (1f / (1f+(0.0002f*(rb.velocity.z+0.1f)))) * RenderSettings.fogDensity;
    }
    if (pc.hit && RenderSettings.fogDensity < startFog) {
      RenderSettings.fogDensity *= 1.05f;
    }
  }
}
