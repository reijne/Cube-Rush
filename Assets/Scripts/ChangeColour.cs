using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColour : MonoBehaviour
{
    private Light l;
    Color next;
    private float fadeTime = 1000f;
    private float tc;
    private int lightTimer = 1000;
    private int colorIndex = 0;
    private Color[] rainbow = {new Color (255,0,0),
                               new Color (255,0,255),
                               new Color (0,0,255),
                               new Color (0,255,255),
                               new Color (0,255,0),
                               new Color (255,255,0)};
    // Start is called before the first frame update
    void Start()
    {
      l = this.GetComponent<Light>();
    }

    void setColor() {
      // next = new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 0.2f);
      next = rainbow[colorIndex];
      colorIndex = (colorIndex + 1) % 6;
      tc = 0;
    }
    // Update is called once per frame
    private void FixedUpdate() {
      lightTimer++;
      if (lightTimer > 1000) {
        setColor();
        lightTimer = 0;
      }
      if (tc <= 1) {
        tc += Time.deltaTime / fadeTime;
        l.color = Color.Lerp(l.color, next, tc);
      }
    }
}
