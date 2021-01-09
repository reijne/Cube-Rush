using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class adManager : MonoBehaviour, IUnityAdsListener
{
  public static adManager instance;
  public static int bonusCounter = 0;
  string placement = "addBonus";

  IEnumerator Start() {
    if (instance != null) {
      Destroy(this);
      yield return null;
    } 
    instance = this;
    DontDestroyOnLoad(this);

    Advertisement.AddListener(this);
    Advertisement.Initialize("3956813", false);
    while (!Advertisement.IsReady(placement))
      yield return null;
  }

  public static void showAd(string placement) {
    bonusCounter++;
    if (bonusCounter >= 5) {
      Advertisement.Show(placement);
      bonusCounter = 0;
      return;
    }
    if (bonusCounter >= 2) {
      if (Random.Range(0, 2) == 1) {
        Advertisement.Show(placement);
        bonusCounter = 0;
      } else {
        GameObject.Find("Player").GetComponent<PlayerCollision>().addBonus();
      }
      return;
    } else {
      GameObject.Find("Player").GetComponent<PlayerCollision>().addBonus();
    }
  }

  public void OnUnityAdsDidError(string message) {
  }

  public void OnUnityAdsDidFinish(string placementId, ShowResult showResult) {
    if (showResult == ShowResult.Finished) {
      GameObject.Find("Player").GetComponent<PlayerCollision>().addBonus();
    }
  }

  public void OnUnityAdsDidStart(string placementId) {
  }

  public void OnUnityAdsReady(string placementId) {
  }
}
