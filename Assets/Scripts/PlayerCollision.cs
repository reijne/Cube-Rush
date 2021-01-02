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
  public GameObject obstacles;
  public GameObject winningScreen;
  public GameObject deathScreen;
  public GameObject increaseDifficultyButton;
  private AudioPlayer audiop;
  private AudioSource soundEffectSource;
  private bool firstFin = true;
  private int hitCooldown;
  private bool adClicked;
  private void Start() {
    GameObject au = GameObject.Find("AudioPlayer");
    audiop = au.GetComponent<AudioPlayer>();
    soundEffectSource = GameObject.Find("DataKeeper").GetComponent<AudioSource>();
    au.GetComponent<AudioSource>().volume = DataKeeper.dataInstance.musicVolume;
    soundEffectSource.GetComponent<AudioSource>().volume = DataKeeper.dataInstance.soundVolume;
    deathScreen.SetActive(false);
    adClicked = false;
  }
  void OnCollisionEnter(Collision c) {
    if (c.collider.tag == "Obstacle") {
      playerHitObstacle(c);
    }
    if (c.collider.tag == "Finish") {
      playerHitFinish(c);
      playFinishSound();  
    }
    if (c.collider.tag == "MovingObstacle") {
      sh.penalty += 50;
      if (!pm.alive) c.rigidbody.AddExplosionForce(10, rb.position, 10, 0, ForceMode.Impulse);
      else if (pm.speed > 100) rb.AddForce(new Vector3(0, 0, -10/DataKeeper.dataInstance.difficulty), ForceMode.Impulse);
    }
  }

  void playHitSound(Collision c) {
    soundEffectSource.clip = audiop.sounds[1];
    soundEffectSource.Play();
  }

  void playFinishSound() {
    if (firstFin) {
      soundEffectSource.clip = audiop.sounds[0];
      soundEffectSource.Play();
      firstFin = false;
    }
  }

  void playerHitFinish(Collision c) {
    pm.alive = false;
    pm.rb.useGravity = false;
    rb.AddForce(0, 0, -10*rb.velocity.z); // Slow the player down if the finish is hit

    if (hit) {
      // notifyText.text = "You finished...\ndont count tho\n\npress R to respawn";
    } else {
      fin = true;

      // Division must be made because otherwise overwriting cross difficulty happens.
      if (DataKeeper.difficultyToString() == "easy") {
        DataKeeper.dataInstance.maxEasyReached = Mathf.Max(DataKeeper.dataInstance.maxEasyReached, pm.currentLevel);
      } else if (DataKeeper.difficultyToString() == "normal") {
        DataKeeper.dataInstance.maxNormalReached = Mathf.Max(DataKeeper.dataInstance.maxNormalReached, pm.currentLevel);
      } else if (DataKeeper.difficultyToString() == "hard") {
        DataKeeper.dataInstance.maxHardReached = Mathf.Max(DataKeeper.dataInstance.maxHardReached, pm.currentLevel);
      } else {
        DataKeeper.dataInstance.maxInsaneReached = Mathf.Max(DataKeeper.dataInstance.maxInsaneReached, pm.currentLevel);
      }

      DataKeeper.save();
      Destroy(groundPlane);
      Destroy(obstacles);
      showWinningScreen();
    }
  }

  public void showWinningScreen(){
    if (SettingsHandler.settingsActive) pm.sh.toggleSettingsMenu(false);
    winningScreen.SetActive(true);
    if (pm.currentLevel == 4) {
      if (!pm.infinite) showPointTotal();
      if (DataKeeper.difficultyToString() == "insane") {
        increaseDifficultyButton.SetActive(false);
      } else {
        increaseDifficultyButton.SetActive(true);
      }
    }
  }

  /*** <summary>Show a textual summary of the player's scores for all levels. </summary>***/
  void showPointTotal() {
    sh.handleBestScores();
    Data loaded = SaveLoad.load();

    if (loaded != null) {
      Dictionary<int, float> bestScores = DataKeeper.getBestScores();
      Text endingText = GameObject.Find("endingText").GetComponent<Text>(); 
      float totalScore = 0;
      endingText.text = "You completed the Challenges on " + DataKeeper.difficultyToString().ToUpper() + " difficulty!\n\n"; 
      for (int i = 0; i <= 4; i++) {
        string spacer = "";
        if (bestScores[i] < 10) spacer += " ";
        if (bestScores[i] < 100) spacer += " ";
        if (bestScores[i] < 1000) spacer += " ";
        endingText.text += "Level " + (i+1) + " :: " + spacer + bestScores[i] + "\n";
        totalScore += bestScores[i]; // TODO fix so it only gets score of proper difficulty
      }
      endingText.text += "====================";
      endingText.text += "\n Total :: " + totalScore + " points";
    }
  }


  void playerHitObstacle(Collision c) {
    if (hitCooldown > 0 || adClicked) return;
    if (DataKeeper.dataInstance.bonus > 0) {
      DataKeeper.dataInstance.bonus--;
    } else {
      pm.alive = false;
      if (!SettingsHandler.settingsActive) deathScreen.SetActive(true);
      hit = true;
    }
    playHitSound(c);
    fp.shake = true;
    hitCooldown = 20;

    float x;
    if (c.impulse.x > 0) {
      x = Mathf.Min(3000f*c.impulse.x, 500f);
    } else {
      x = Mathf.Max(3000f*c.impulse.x, -500f);
    }
    rb.AddForce(x, 10*c.impulse.y, 10*c.impulse.z);
  }

  private void FixedUpdate() {
    if (hitCooldown > 0) hitCooldown--;
  }

  public void showAddBonusAd() {
    adClicked = true;
    adManager.showAd("addBonus");
  }

  public void addBonus() { // Needs to also show an ad and only add a bonus if the video is complete
    if (DataKeeper.dataInstance.bonus == 0) {
      DataKeeper.dataInstance.bonus = 1;
    }
    DataKeeper.dataInstance.bonus++;
    if (pm.infinite) pm.initBlackHole();
  }
}
