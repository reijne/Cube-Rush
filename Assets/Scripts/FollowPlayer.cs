using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
  public Transform player;
  private Vector3 offset = new Vector3(0, 2f, -5f);
  public PlayerCollision pc;
  public bool shake = false;
  private int shakeCount = 0;
  private float yoff = 0.004f;
  private float zoff = 0.0042f;

  // Update is called once per frame
  void Update() {
    transform.position = player.position + offset;
    if (shake && shakeCount < 20) {
      transform.position += (shakeCount % 3 - 1) * new Vector3(0.04f, 0.04f, 0.04f);
    }
  }

  private void FixedUpdate() {
    if (pc.fin) {
      if (DataKeeper.android) {
        offset += new Vector3(0, yoff, -zoff/2) / DataKeeper.androidOffset;
      } else {
        offset += new Vector3(0, yoff, -zoff) / (DataKeeper.dataInstance.difficulty+1);
      }
    }
    if (shake) {
      shakeCount++;
    } 
  }
}
