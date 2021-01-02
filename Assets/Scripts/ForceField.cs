using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
  // Start is called before the first frame update
  public static AudioSource wub;
  private static AudioPlayer audiop;
  private void Start() {
    if (wub == null) {
      wub = GameObject.Find("DataKeeper").GetComponent<AudioSource>();
      wub.Stop();
      audiop = GameObject.Find("AudioPlayer").GetComponent<AudioPlayer>();
      wub.clip = audiop.sounds[2];
    }
  }
  private void OnCollisionEnter(Collision c) {
    if (c.gameObject.tag == "MovingObstacle" && PlayerMovement.fireForceField) {
      Destroy(c.gameObject);
      PlayerMovement.growForceField = true;
    }
  }

  public static void playWub() {
    if (wub.clip != audiop.sounds[2]) wub.clip = audiop.sounds[2]; 
    wub.Play();
  }
}
