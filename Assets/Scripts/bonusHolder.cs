using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bonusHolder : MonoBehaviour
{
  public PlayerMovement pm;
  public GameObject shieldImage;
  public GameObject circleImage;
  public Text bonusCount;
  private void FixedUpdate() {
    if (pm.infinite) {
      if (pm.blackHoleCount > 0) {
        shieldImage.SetActive(false);
        circleImage.SetActive(true);
        bonusCount.text = pm.blackHoleCount.ToString();
      } else {
        wipe();
      }
    } else {
      if (DataKeeper.dataInstance.bonus > 0) {
      shieldImage.SetActive(true);
      circleImage.SetActive(false);
      bonusCount.text = DataKeeper.dataInstance.bonus.ToString();
      } else {
        wipe();
      }
    }
  }

  private void wipe() {
    shieldImage.SetActive(false);
    circleImage.SetActive(false);
    bonusCount.text = "";
  }
}
