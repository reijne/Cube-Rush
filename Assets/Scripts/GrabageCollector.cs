using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabageCollector : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision c) {
      Destroy(c.gameObject);
    }
}
