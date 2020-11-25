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
    // Start is called before the first frame update
  void Start() {
  }

  // Update is called once per frame
  void Update() {
    transform.position = player.position + offset;
    if (shake && shakeCount < 15) {
      transform.position += (shakeCount % 3 - 1) * new Vector3(0.04f, 0.04f, 0.04f);
    }
  }

  private void FixedUpdate() {
    if (pc.fin) {
      offset += new Vector3(0, 0.004f, -0.042f);
    }
    if (shake) {
      shakeCount++;
    } 
  }
}
