using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadGame : MonoBehaviour
{
  private void Awake() {
    Invoke("loadTitle", 0.01f);
  }

  private void loadTitle() {
    StartCoroutine(DataKeeper.loadAsyncy("title"));
  }
}
