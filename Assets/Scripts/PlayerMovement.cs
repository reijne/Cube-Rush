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
    float horizontalInput;
    bool android = false;
    public GameObject androidControls;
    private float moveMult = 1;
    // float horizontalInput = Input.GetAxis ("Horizontal"); 
    // Start is called before the first frame update
    void Start()
    {
        timeRoundStart = Time.time;
        if (!android) {
          Destroy(androidControls.gameObject);
          // Debug.Log("androidControls destroyed");
        }
        moveMult = 1 / moveMult;
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
            SceneManager.LoadScene((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());
            pc.fin = false;
          }
          // Debug.Log("Detected key code: " + e.keyCode);
        }
    }

    // Update is called once per frame
    void Update() {
      if (android) {
        handleTouch();
      } else {
        horizontalInput = Input.GetAxisRaw("Horizontal");
      }
      if (rb.transform.position.y <= -4) {
        pc.restartText.text = "press R to respawn";
        alive = false;
      }
    }
    // Fixed time update
    void FixedUpdate()
    {
      jumpTimer++;
        
      if (alive) {
        // Move the player along the the z-axis and handle the horizontal input for x-axis.
        if (!infinite) {
          rb.AddForce(0, 0, moveMult * forwardForce * Time.deltaTime * (Time.time - timeRoundStart) * 0.5f);
        } else {
          speed = 20 * Mathf.Sqrt(Time.time - timeRoundStart);
          if (rb.transform.position.z < -40) {
            rb.AddForce(0, 0, 0.5f*speed);
          }
          if (rb.transform.position.y > 2) {
            rb.AddForce(0, -0.1f*speed, 0);
            rb.AddForce(0, 0, -0.5f*speed);
          }
        }
        rb.AddForce(moveMult * horizontalInput * sideForce * Time.deltaTime, 0, 0, ForceMode.VelocityChange);
      }
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
