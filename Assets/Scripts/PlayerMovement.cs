using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public PlayerCollision pc;
    public Rigidbody rb;
    public Joystick joystick;
    public float forwardForce = 200f;
    public float sideForce = 560f;
    public float jumpForce = 5f;
    public bool alive = true;
    public int jumpTimer = 69;
    // public bool justJumped = false;
    public float timeRoundStart;
    public bool infinite = true;
    public float speed = 10;
    public float horizontalInput;
    public GameObject androidControls;
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
    private bool fireForceField = false;
    private float percent;
    private AudioSource music;
    private bool musicStarted = false;
    private int holeCount;
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
      music = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
      if (!DataKeeper.android) {
        Destroy(androidControls.gameObject);
      }
      if (infinite) {
        float sizer = (DataKeeper.dataInstance.difficulty + 20) / 10;
        normalScale = new Vector3(sizer, sizer, sizer);
        forcefieldobj.transform.localScale = normalScale;
        forcefieldobj.transform.position = new Vector3(0, -10, 1);
        forcefieldmesh = forcefieldobj.GetComponent<MeshRenderer>();
        StartCoroutine(infiniteStartText());
      }
      moveMult = 1f / DataKeeper.dataInstance.difficulty;
      // also scale forceField by difficulty
      forceFieldCooldownInit = (int) (forceFieldCooldownInit / (5 * DataKeeper.dataInstance.difficulty));
      forceFieldCooldown = 300;
      holeCount = (int) (3 * DataKeeper.dataInstance.difficulty);
    }

    IEnumerator infiniteStartText() {
      pc.notifyText.text = "Survive till the end of the song";
      yield return new WaitForSeconds(2);
      pc.notifyText.text += "\n\nSelect a different song in settings (esc)";
      yield return new WaitForSeconds(4);
      pc.notifyText.text = "Release a black hole using `W`, `Up arrow` or click with the mouse";
      holeCount++;
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
        if (e.keyCode == KeyCode.Space && jumpTimer >= 69 && alive) {
          rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
          jumpTimer = 0;
        }
        if (e.keyCode == KeyCode.Return && pc.fin) {
          nextLevel();
        }
        if (e.keyCode == KeyCode.Escape) {
          if (!infinite) quit();
          // if (!infinite) SceneManager.LoadScene("title");
        }
        // Debug.Log("Detected key code: " + e.keyCode);
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
      StartCoroutine(DataKeeper.loadAsyncy("title"));
    }

    public void increaseDifficultyRestart() {
      if (DataKeeper.difficultyToString() == "easy") {
        DataKeeper.dataInstance.difficulty = 2.5f;
      } else if (DataKeeper.difficultyToString() == "normal") {
        DataKeeper.dataInstance.difficulty = 1;
      } else if (DataKeeper.difficultyToString() == "hard") {
        DataKeeper.dataInstance.difficulty = 0.7f;
      }
      SceneManager.LoadScene("0");
    }

    public void startSurvival() {
      SceneManager.LoadScene("5");
    }

    // Update is called once per frame
    void Update() {
      if (DataKeeper.android) {
        handleTouch();
      } else {
        horizontalInput = Input.GetAxisRaw("Horizontal");
      }
      if (rb.transform.position.y <= -4) {
        pc.notifyText.text = "press R to respawn";
        alive = false;
      }
      if (infinite) {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && forceFieldCooldown == 0 && holeCount > 0) {
          engageForceField();
        }
      }
    }

    private void engageForceField() {
      fireForceField = true;
      forceFieldCooldown = forceFieldCooldownInit;
      forcefieldobj.transform.position = rb.transform.position + new Vector3(0, 0, forcefieldobj.transform.localScale.z + 0.5f);
      ForceField.playWub();
      holeCount--;
    }

    // Fixed time update
    void FixedUpdate()
    {
      jumpTimer++;
      // jumpTimer = 69;
      
      if (alive) {
        // Move the player along the the z-axis and handle the horizontal input for x-axis.
        if (!infinite) {
          rb.AddForce(0, 0, moveMult * forwardForce * Time.deltaTime * (Time.time - timeRoundStart) * 0.5f);
        } else { //infinite
          speed = (20 * Mathf.Sqrt(Time.time - timeRoundStart)) / DataKeeper.dataInstance.difficulty;
          if (forceFieldCooldown <= (forceFieldCooldownInit / 2)) charge();
          infiniteMovementHandler();
          abilityHandler();
          musicHandler();
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
        rb.AddForce(0, -0.1f*speed, 0); //plop player back on platform
        rb.AddForce(0, 0, -0.5f*speed); //give penalty cuz 2 high
      }
    }
    private void abilityHandler() {
      if (forceFieldCooldown > 0 && holeCount > 0) { // Handle the forceField/black hole
        forceFieldCooldown--;
        percent = 100 - (float) forceFieldCooldown / forceFieldCooldownInit * 100;
        if (forceFieldCooldown < 0) forceFieldCooldown = 0;
        forceFieldText.text = "Black Hole :: " + holeCount + " (" + Mathf.Round(percent) + ")";
      } else if (holeCount > 0) {
        forceFieldText.text = "Black Hole :: " + holeCount + " (READY)";
      } else {
        forceFieldText.text = "Black Hole :: ---";
      }
    }

    private void musicHandler() {
      if (music.isPlaying && !musicStarted && currentLevel == 5) {// Music hath engaged
        musicStarted = true;
      }
      if (!music.isPlaying && musicStarted && alive) { // Music is finito -> Player wins survival
        alive = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        string songName = music.clip.ToString().Split('(')[0];
        pc.notifyText.text = "You completed\n\n" + songName + 
                              "\n\n" + DataKeeper.difficultyToString() + " :: " + pc.sh.convertScore(pc.sh.score) + " points";
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

      if (Input.touchCount > 0) {
        if (pc.fin) {
          SceneManager.LoadScene((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());
          pc.fin = false;
        } else if (!alive) {
          SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } else if (Input.GetTouch(0).position.x > Screen.width / 2) {
          horizontalInput = 1;
        } else if (Input.GetTouch(0).position.x < Screen.width / 2) {
          horizontalInput = -1;
        }
      }
      // horizontalInput = joystick.Horizontal;
      // Debug.Log(horizontalInput);
    }
}
