using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandling : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scoreObj;
    public PlayerMovement pm;
    public float penalty = 0;
    public float score = 0;

    // Update is called once per frame
    private void FixedUpdate() {
      Debug.Log(penalty);
      if (penalty <= 0 && pm.alive) {
        score += (Time.time - pm.timeRoundStart);
        // Debug.Log("Added: " + (Time.time - pm.timeRoundStart));
      } else {
        Debug.Log("Penalty");
        penalty--;
      }
      scoreObj.text = Mathf.Round(score).ToString(); 
    }
}
