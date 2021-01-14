using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bonusHolder : MonoBehaviour
{
  public PlayerMovement pm;
  public GameObject shieldImageObj;
  public Image shieldImage;
  public GameObject circleImageObj;
  public Image circleImage;
  public Text bonusCount;
  private int localCounter;
  private int showRed = 0;

  private void Start() {
    localCounter = DataKeeper.dataInstance.bonus;
  }
  private void FixedUpdate() {
    if (showRed > 0) showRed--;
    if (pm.infinite) {
      if (pm.blackHoleCount > 0) {
        shieldImageObj.SetActive(false);
        circleImageObj.SetActive(true);
        bonusCount.text = pm.blackHoleCount.ToString();
        checkBonusLoss();
      } else {
        checkBonusLoss();
        wipe();
      }
    } else {
      if (DataKeeper.dataInstance.bonus > 0) {
        shieldImageObj.SetActive(true);
        circleImageObj.SetActive(false);
        bonusCount.text = DataKeeper.dataInstance.bonus.ToString();
        checkBonusLoss();
      } else {
        checkBonusLoss();
        if (showRed == 0) wipe();
      }
    }
  }

  private void checkBonusLoss() {
    if (DataKeeper.dataInstance.bonus < localCounter) {
      if (pm.infinite) {
        circleImage.color = Color.red;
      } else {
        shieldImage.color = Color.red;
      }
      localCounter = DataKeeper.dataInstance.bonus;
      showRed = 32;
    } else if (showRed == 0) {
      if (pm.infinite) {
        circleImage.color = Color.white;
      } else {
        shieldImage.color = Color.white;
      }
    }
  }

  private void wipe() {
    shieldImageObj.SetActive(false);
    circleImageObj.SetActive(false);
    bonusCount.text = "";
  }
}
