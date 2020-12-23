using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
  // Start is called before the first frame update
  public static AudioSource wub;
  private void Awake() {
    if (wub == null) {
      wub = GetComponent<AudioSource>();
      wub.Stop();
    } else {
      Destroy(this);
    }
  }
  private void OnCollisionEnter(Collision c) {
    if (c.gameObject.tag == "MovingObstacle") {
      Destroy(c.gameObject);
      PlayerMovement.growForceField = true;
    }
  }

  public static void playWub() {
    wub.Play();
  }
}
