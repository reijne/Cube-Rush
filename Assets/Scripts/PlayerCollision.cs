using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerCollision : MonoBehaviour
{
  public static int freeCollisions = 0;
  public PlayerMovement pm;
  public ScoreHandling sh;
  public FollowPlayer fp;
  public Rigidbody rb;
  public Camera cam;
  public Text notifyText;
  public bool hit = false;
  public bool fin = false;
  public GameObject groundPlane;
  private AudioPlayer audiop;
  private AudioSource audiosrc;
  private bool firstFin = true;
  private bool firstHit = true;
  
  private void Start() {
    audiop = GameObject.Find("AudioPlayer").GetComponent<AudioPlayer>();
    audiosrc = audiop.GetComponent<AudioSource>();
  }
  void OnCollisionEnter(Collision c) {
    if (c.collider.tag == "Obstacle") {
      playerHitObstacle(c);
      playHitSound(c);
    }
    if (c.collider.tag == "Finish") {
      playerHitFinish(c);
      playFinishSound();  
    }
    if (c.collider.tag == "MovingObstacle") {
      sh.penalty += 50;
      rb.AddForce(new Vector3(0, 0, -10), ForceMode.Impulse);
    }
  }

  void playHitSound(Collision c) {
    if (!firstHit) return;
    audiosrc.clip = audiop.sounds[1];
    audiosrc.Play();
    firstHit = false;
  }

  void playFinishSound() {
    if (firstFin) {
      audiosrc.clip = audiop.sounds[0];
      audiosrc.Play();
      firstFin = false;
    }
  }

  void playerHitFinish(Collision c) {
    pm.alive = false;
    pm.rb.useGravity = false;
    rb.AddForce(0, 0, -10*rb.velocity.z); // Slow the player down if the finish is hit

    if (hit) {
      notifyText.text = "You finished...\ndont count tho\n\npress R to respawn";
    } else {
      notifyText.text = "You finished!\n\n";
      if (pm.currentLevel < 4) notifyText.text += "'Enter' to continue";
      fin = true;

      // Division must be made because otherwise overwriting cross difficulty happens.
      if (DataKeeper.difficultyToString() == "normal") {
        DataKeeper.dataInstance.maxNormalReached = Mathf.Max(DataKeeper.dataInstance.maxNormalReached, pm.currentLevel);
      } else if (DataKeeper.difficultyToString() == "hard") {
        DataKeeper.dataInstance.maxHardReached = Mathf.Max(DataKeeper.dataInstance.maxHardReached, pm.currentLevel);
      } else {
        DataKeeper.dataInstance.maxInsaneReached = Mathf.Max(DataKeeper.dataInstance.maxInsaneReached, pm.currentLevel);
      }

      DataKeeper.save();
      Destroy(groundPlane);
      if (pm.currentLevel == 4) showPointTotal();
    }
  }

  /*** <summary>Show a textual summary of the player's scores for all levels. </summary>***/
  void showPointTotal() {
    sh.handleBestScores();
    Data loaded = SaveLoad.load();

    if (loaded != null) {
      Dictionary<int, float> bestScores = DataKeeper.getBestScores(); 
      float totalScore = 0;
      notifyText.text = "You completed the Challenges on " + DataKeeper.difficultyToString() + " difficulty!\n\n"; 
      for (int i = 0; i <= 4; i++) {
        string spacer = "";
        if (bestScores[i] < 10) spacer += " ";
        if (bestScores[i] < 100) spacer += " ";
        if (bestScores[i] < 1000) spacer += " ";
        notifyText.text += "Level " + (i+1) + " :: " + spacer + bestScores[i] + "\n";
        totalScore += bestScores[i]; // TODO fix so it only gets score of proper difficulty
      }
      notifyText.text += "====================";
      notifyText.text += "\n Total :: " + totalScore + " points";
    }
  }


  void playerHitObstacle(Collision c) {
      pm.alive = false;
      notifyText.text = "press R to respawn";
      if (DataKeeper.android) {
        notifyText.text = "tap to respawn";  
      } 
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
