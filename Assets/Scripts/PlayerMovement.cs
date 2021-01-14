using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public PlayerCollision pc;
    public SettingsHandler sh;
    public Rigidbody rb;
    public float forwardForce = 200f;
    public float sideForce = 560f;
    public float jumpForce = 5f;
    public bool alive = true;
    public int jumpTimer = 69;
    public bool justJumped = false;
    public float timeRoundStart;
    public bool infinite = true;
    public float speed = 10;
    public float horizontalInput;
    public GameObject androidControls;
    public GameObject controlTutorial;
    public GameObject forcefieldobj;
    public MeshRenderer forcefieldmesh;
    private float moveMult = 1;
    public int currentLevel;
    public int maxSize = 10;
    private Vector3 normalScale;
    public Text forceFieldText;
    public int forceFieldCooldownInit;
    private int forceFieldCooldown;
    public static bool growForceField = false;
    public static bool fireForceField = false;
    private float percent;
    private AudioSource musicSource;
    private AudioPlayer audiop;
    private bool musicStarted = false;
    public int blackHoleCount;
    public static bool startSpeedIncrement = false;
    // float horizontalInput = Input.GetAxis ("Horizontal"); 

    private void Awake() {
      currentLevel = int.Parse(SceneManager.GetActiveScene().name);
    }

    // Start is called before the first frame update
    void Start()
    {
      // forcefieldobj.GetComponent<MeshRenderer>().enabled = false;
      timeRoundStart = Time.time;
      musicSource = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
      if (DataKeeper.android) {
        moveMult = 1f / (DataKeeper.dataInstance.difficulty+DataKeeper.androidOffset);
        if (DataKeeper.difficultyToString() == "easy" && currentLevel == 0) {
        Debug.Log("starting to show controls");
        StartCoroutine(showControlTutorial());
      }
      } else {
        Destroy(androidControls.gameObject);
        moveMult = 1f / DataKeeper.dataInstance.difficulty;
      }
      if (infinite) {
        float sizer = (DataKeeper.dataInstance.difficulty + 20) / 10;
        normalScale = new Vector3(sizer, sizer, sizer);
        forcefieldobj.transform.localScale = normalScale;
        forcefieldobj.transform.position = new Vector3(0, -10, 1);
        forcefieldmesh = forcefieldobj.GetComponent<MeshRenderer>();
        sh.addSongs();
        StartCoroutine(infiniteStartText());
      } else {
        audiop = GameObject.Find("AudioPlayer").GetComponent<AudioPlayer>();
        if (!musicSource.isPlaying || musicSource.clip != audiop.songs[audiop.songs.Count-1]) {
          startThemeSong();
        }
      }
      // also scale forceField by difficulty
      forceFieldCooldownInit = (int) (forceFieldCooldownInit / (4 * DataKeeper.dataInstance.difficulty));
      forceFieldCooldown = 300;
      adManager.bonusCounter = 0;
      initBlackHole();
      if (DataKeeper.dataInstance.bonus == 0) {
        DataKeeper.dataInstance.bonus = 1;
      }
    }
    IEnumerator showControlTutorial() {
      Debug.Log("currently showing controls");
      controlTutorial.SetActive(true);
      yield return new WaitForSeconds(2);
      Debug.Log("done showing controls");
      controlTutorial.SetActive(false);
      yield return null;
    }

    public void startThemeSong() {
      musicSource.clip = audiop.songs[audiop.songs.Count-1];
      musicSource.Play();
      musicSource.loop = true;
    }

    public void initBlackHole() {
      blackHoleCount = (int) (Mathf.Round(DataKeeper.dataInstance.difficulty+0.1f) + DataKeeper.dataInstance.bonus);
      fireForceField = false;
    }

    IEnumerator infiniteStartText() {
      if (DataKeeper.dataInstance.selectedMusic != "Infinite Silence") {
        pc.notifyText.text = "Survive till the end of the song";
      } else {
        pc.notifyText.text = "Survive till the end of time";
      }
      yield return new WaitForSeconds(2);
      pc.notifyText.text += "\n\nDifferent songs are in settings";
      if (!DataKeeper.android) pc.notifyText.text += " (esc)";  
      yield return new WaitForSeconds(4);
      pc.notifyText.text = "Release black hole";
      if (DataKeeper.android) pc.notifyText.text += " with black button";
      else pc.notifyText.text += "\n`W`, `Up arrow` or click (LMB)";
      blackHoleCount++;
      DataKeeper.dataInstance.bonus++;
      engageForceField();
      yield return new WaitForSeconds(2);
      pc.notifyText.text = "";
    }

    /* Handle key-presses by user accordingly. */
    void OnGUI() {
      Event e = Event.current;
      if (e.isKey) {
        if (e.keyCode == KeyCode.R) {
          restart();
        }
        if (e.keyCode == KeyCode.Space) {
          jump();
        }
        if (e.keyCode == KeyCode.Return && pc.fin) {
          nextLevel();
        }
        // if (e.keyCode == KeyCode.Escape) {
        //   if (!infinite) quit();
        //   // if (!infinite) SceneManager.LoadScene("title");
        // }
        // Debug.Log("Detected key code: " + e.keyCode);
      }
    }

    public void jump() {
      if (jumpTimer >= 69 && alive) {
        rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        jumpTimer = 0;
      }
    }

    public void restart() {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload current scene
    }

    public void nextLevel() {
      if (currentLevel < 4) {
        SceneManager.LoadScene((currentLevel + 1).ToString());
        pc.fin = false;
      }
    }

    public void quit() {
      SceneManager.LoadScene("title");
      // StartCoroutine(DataKeeper.loadAsyncy("title"));
    }

    public void increaseDifficultyRestart() {
      if (DataKeeper.difficultyToString() == "easy") {
        DataKeeper.dataInstance.difficulty = 2.5f;
      } else if (DataKeeper.difficultyToString() == "normal") {
        DataKeeper.dataInstance.difficulty = 1.25f;
      } else if (DataKeeper.difficultyToString() == "hard") {
        DataKeeper.dataInstance.difficulty = 0.7f;
      }
      if (infinite) SceneManager.LoadScene("5"); 
      else SceneManager.LoadScene("0");
    }

    public void startSurvival() {
      SceneManager.LoadScene("5");
    }

    public void startChallenges() {
      SceneManager.LoadScene("0");
    }

    // Update is called once per frame
    void Update() {
      if (DataKeeper.android) {
        handleTouch();
      } else {
        horizontalInput = Input.GetAxisRaw("Horizontal");
      }
      if (rb.transform.position.y <= -4) {
        if (!SettingsHandler.settingsActive) pc.deathScreen.SetActive(true);;
        alive = false;
      }
      if (infinite) {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !DataKeeper.android) {
          engageForceField();
        }
      }
    }

    public void engageForceField() { // link black hole button to dis
      if (forceFieldCooldown == 0 && blackHoleCount > 0 && !SettingsHandler.settingsActive && !pc.deathScreen.activeSelf && !pc.winningScreen.activeSelf) {
        fireForceField = true;
        forceFieldCooldown = forceFieldCooldownInit;
        forcefieldobj.transform.position = rb.transform.position + new Vector3(0, 0, forcefieldobj.transform.localScale.z + 0.5f);
        ForceField.playWub();
        blackHoleCount--;
        if (DataKeeper.dataInstance.bonus > 0) {
          DataKeeper.dataInstance.bonus--;
        }    
      }
    }

    // Fixed time update
    void FixedUpdate()
    {
      jumpTimer++;
      // jumpTimer = 69;
      
      if (alive) {
        // Move the player along the the z-axis and handle the horizontal input for x-axis.
        if (!infinite) { // challenges
          rb.AddForce(0, 0, moveMult * forwardForce * Time.deltaTime * (Time.time - timeRoundStart) * 0.5f);
          if (rb.position.y > 2.5f) {
            rb.AddForce(new Vector3(0, -2*rb.velocity.y, 0), ForceMode.VelocityChange);
          }
        } else { //infinite
          if (DataKeeper.android) {
            speed = (20 * Mathf.Sqrt(Time.time - timeRoundStart)) / (DataKeeper.dataInstance.difficulty+DataKeeper.androidOffset);
          } else {
            speed = (20 * Mathf.Sqrt(Time.time - timeRoundStart)) / DataKeeper.dataInstance.difficulty;
          }
          if (forceFieldCooldown <= (forceFieldCooldownInit / 2)) charge();
          infiniteMovementHandler();
          abilityHandler();
          musicSourceHandler();
        }
        rb.AddForce(moveMult * horizontalInput * sideForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
      }

      if (fireForceField) {
        fiyah();
      }      
    }

    private void infiniteMovementHandler() {
      if (rb.transform.position.z < -40) {
        rb.AddForce(0, 0, 0.5f*speed); //move player back to start
      }
      if (rb.transform.position.y > 2) { 
        rb.AddForce(0, -0.1f*speed, 0, ForceMode.VelocityChange); //plop player back on platform
        rb.AddForce(0, 0, -0.5f*speed); //give penalty cuz 2 high
      }
    }
    private void abilityHandler() {
      if (forceFieldCooldown > 0 && blackHoleCount > 0) { // Handle the forceField/black hole
        forceFieldCooldown--;
        percent = 100 - (float) forceFieldCooldown / forceFieldCooldownInit * 100;
        if (percent < 0) percent = 0;
        forceFieldText.text = "Black Hole :: (" + Mathf.Round(percent) + ")";
      } else if (blackHoleCount > 0) {
        forceFieldText.text = "Black Hole :: (READY)";
      } else {
        forceFieldText.text = "Black Hole :: ---";
      }
    }

    private void musicSourceHandler() {
      if (musicSource.isPlaying && !musicStarted && currentLevel == 5) {// musicSource hath engaged
        musicStarted = true;
      }
      if (!musicSource.isPlaying && musicStarted && alive && Application.isFocused) { // musicSource is finito -> Player wins survival
        alive = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        string songName = musicSource.clip.ToString().Split('(')[0];
        pc.notifyText.text = songName +"\n\n" + DataKeeper.difficultyToString().ToUpper() + " :: " 
                            + pc.sh.convertScore(pc.sh.score) + " points";
        pc.showWinningScreen();
        pc.fin = true;
        sh.forceRestart = true;
        if (SettingsHandler.settingsActive) sh.toggleSettingsMenu(true);
      }
    }

    private void fiyah() {
      forcefieldobj.transform.position += new Vector3(0, 0, 1);
      if (forcefieldobj.transform.localScale.z <= maxSize && growForceField) {
        forcefieldobj.transform.localScale += new Vector3(0.5f, 0.5f, 1f);
      }
      growForceField = false;
    }

    private void charge() {
      forcefieldobj.transform.position = rb.position - new Vector3(0, 0, 1.5f);
      if (percent >= 95 && percent < 100) forcefieldmesh.material.color = Color.white;
      else if (percent == 100) forcefieldmesh.material.color = Color.black;
      forcefieldobj.transform.localScale = normalScale * (percent-50) / 100;
      fireForceField = false;
    }

    /* Handle touches of the screen if android/ios is used. */
    void handleTouch() {
      if (Input.touchCount > 2) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return;
      }
      horizontalInput = 0;
      if (Input.touchCount > 0 && alive) {
        if (Input.GetTouch(0).position.y < Screen.height / 4 || Input.GetTouch(0).position.y > Screen.height - Screen.height / 6) return;
        if (Input.GetTouch(0).position.x > Screen.width / 2) {
          horizontalInput = 1;
        } else if (Input.GetTouch(0).position.x < Screen.width / 2) {
          horizontalInput = -1;
        }
      }
      // horizontalInput = joystick.Horizontal;
      // Debug.Log(horizontalInput);
    }
}
