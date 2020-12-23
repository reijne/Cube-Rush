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
    private float moveMult = 1;
    public int currentLevel;
    public int maxSize = 10;
    private Vector3 normalScale;
    public Text forceField;
    public int forceFieldCooldownInit;
    private int forceFieldCooldown;
    public static bool growForceField = false;
    private bool fireForceField = false;
    private float percent;
    private AudioSource music;
    private bool musicStarted = false;
    // float horizontalInput = Input.GetAxis ("Horizontal"); 
    // Start is called before the first frame update
    void Start()
    {
      // forcefieldobj.GetComponent<MeshRenderer>().enabled = false;
      currentLevel = int.Parse(SceneManager.GetActiveScene().name);
      timeRoundStart = Time.time;
      music = GameObject.Find("AudioPlayer").GetComponent<AudioSource>();
      if (!DataKeeper.android) {
        Destroy(androidControls.gameObject);
      }
      if (infinite) {
        float sizer = (DataKeeper.dataInstance.difficulty + 5) / 3;
        normalScale = new Vector3(sizer, sizer, sizer);
        forcefieldobj.transform.localScale = normalScale;
        forcefieldobj.transform.position = new Vector3(0, -10, 1);
      }
      moveMult = 1f / DataKeeper.dataInstance.difficulty;
      // also scale forceField by difficulty
      forceFieldCooldownInit = (int) (forceFieldCooldownInit / DataKeeper.dataInstance.difficulty);
      forceFieldCooldown = forceFieldCooldownInit / 2;
    }

    /* Handle key-presses by user accordingly. */
    void OnGUI() {
      Event e = Event.current;
      if (e.isKey) {
        if (e.keyCode == KeyCode.R) {
          SceneManager.LoadScene(SceneManager.GetActiveScene().name); // reload current scene
        }
        if (e.keyCode == KeyCode.Space && jumpTimer >= 69 && alive) {
          rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
          jumpTimer = 0;
        }
        if (e.keyCode == KeyCode.Return && pc.fin) {
          if (currentLevel < 4) {
            SceneManager.LoadScene((currentLevel + 1).ToString());
            pc.fin = false;
          }
        }
        if (e.keyCode == KeyCode.Escape) {
          if (!infinite) SceneManager.LoadScene("title");
        }
        // Debug.Log("Detected key code: " + e.keyCode);
      }
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
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q)) && forceFieldCooldown == 0) {
          fireForceField = true;
          forceFieldCooldown = forceFieldCooldownInit;
          forcefieldobj.transform.position = rb.transform.position + new Vector3(0, 0, forcefieldobj.transform.localScale.z + 0.5f);
          ForceField.playWub();
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
      if (forceFieldCooldown > 0) { // Handle the forceField/black hole
        forceFieldCooldown--;
        percent = 100 - (float) forceFieldCooldown / forceFieldCooldownInit * 100;
        forceField.text = "Black Hole :: " + Mathf.Round(percent);
      } else {
        forceField.text = "Black Hole :: READY";
      }
    }

    private void musicHandler() {
      if (music.isPlaying && !musicStarted) {// Music hath engaged
        musicStarted = true;
      }
      if (!music.isPlaying && musicStarted && alive) { // Music is finito -> Player wins survival
        alive = false;
        string songName = music.clip.ToString().Split('(')[0];
        pc.notifyText.text = "You completed\n\n" + songName + 
                              "\n\n" + DataKeeper.difficultyToString() + " :: " + pc.sh.convertScore() + " points";
      }
    }

    private void fiyah() {
      Debug.Log("Fire command received and playing out");
      forcefieldobj.transform.position += new Vector3(0, 0, 1);
      if (forcefieldobj.transform.localScale.z <= maxSize && growForceField) {
        Debug.Log("Growing force field");
        forcefieldobj.transform.localScale += new Vector3(0.5f, 0.5f, 1f);
      }
      growForceField = false;
    }

    private void charge() {
      Debug.Log("Back to start..... ++ CHARGING");
      forcefieldobj.transform.position = rb.position - new Vector3(0, 0, 1.5f);
      forcefieldobj.transform.localScale = normalScale * (percent-50) / 100;
      fireForceField = false;
    }

    /* Handle touches of the screen if android/ios is used. */
    void handleTouch() {
      if (Input.touchCount > 2) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return;
      }
      // This for loop should be used to move the player maybe?
      for (int i = 0; i < Input.touchCount; ++i) {
          if (Input.GetTouch(i).phase == TouchPhase.Began) {
              if (pc.fin) {
                SceneManager.LoadScene((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());
                pc.fin = false;
              } else if (!alive) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
              }
          }
      }
      horizontalInput = joystick.Horizontal;
      // Debug.Log(horizontalInput);
    }
}
