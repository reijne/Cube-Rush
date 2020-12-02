using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{   
    public PlayerMovement pm;
    public Rigidbody rb;
    
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
      player = GameObject.Find("Player");
      pm = player.GetComponent<PlayerMovement>();
      rb = this.GetComponent<Rigidbody>();
      rb.velocity = new Vector3(0, 0, -pm.speed);
    }

    private void FixedUpdate() {
      rb.velocity = new Vector3(0, 0, -pm.speed);
    }

    private void OnCollisionEnter(Collision c) {
      if (c.gameObject.tag == "MovingObstacle" && c.gameObject.transform.position.z > 10) {
        Destroy(c.gameObject);
      }
    }
}
