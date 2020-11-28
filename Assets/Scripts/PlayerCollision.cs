using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerCollision : MonoBehaviour
{
  public PlayerMovement pm;
  public ScoreHandling sh;
  public FollowPlayer fp;
  public Rigidbody rb;
  public Camera cam;
  public Text finishText;
  public Text finishExtra;
  public Text restartText;
  public bool hit = false;
  public bool fin = false;
  public GameObject groundPlane;
  
  void OnCollisionEnter(Collision c) {
    if (c.collider.tag == "Obstacle") {
      playerHitObstacle(c);
    }
    if (c.collider.tag == "Finish") {
      playerHitFinish(c);  
    }
    if (c.collider.tag == "MovingObstacle") {
      sh.penalty += 50;
    }
  }

  void playerHitFinish(Collision c) {
    pm.alive = false;
    pm.rb.useGravity = false;
    Debug.Log(rb.velocity.z);
    rb.AddForce(0, 0, -10*rb.velocity.z); // Slow the player down if the finish is hit

    if (hit) {
      finishText.text = "You finished...";
      finishExtra.text = "dont count tho";
      restartText.text = "press R to respawn";
    } else {
      finishText.text = "You finished!";
      restartText.text = "'Enter' to continue";
      fin = true;
      Destroy(groundPlane);
    }
  }

  void playerHitObstacle(Collision c) {
      pm.alive = false;
      restartText.text = "press R to respawn";
      hit = true;
      fp.shake = true;

      float x;
      if (c.impulse.x > 0) {
        x = Mathf.Min(3000f*c.impulse.x, 500f);
      } else {
        x = Mathf.Max(3000f*c.impulse.x, -500f);
      }
      rb.AddForce(x, 10*c.impulse.y, 10*c.impulse.z);
  }
}
