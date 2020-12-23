using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataKeeper : MonoBehaviour
{
  public static Data dataInstance;
  public static bool android = false;
  /* Get a load from file, or create a new dataInstance to store all needed information*/
  private void Awake() {
    if (dataInstance == null) {
      Data loaded = load();
      if (loaded != null) {
        dataInstance = loaded;
      } else {
        dataInstance = new Data();
        SaveLoad.save(dataInstance);
      }
    } else {
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);
  }

  public static Data load() {
    Data loaded = SaveLoad.load();
    if (loaded != null) {
      dataInstance = loaded;
    }
    return loaded;
  }

  public static void save() {
    if (dataInstance != null) {
      SaveLoad.save(dataInstance);
    } else {
      Debug.LogWarning("Should never get here, awakening the datakeeper has failed somehow");
      dataInstance = new Data();
      SaveLoad.save(dataInstance);
    }
  }

  public static string difficultyToString() {
    if (dataInstance == null) return "";
    if (dataInstance.difficulty == 2) return "normal";
    else if (dataInstance.difficulty == 1) return "hard";
    else return "insane";
  }

  public static Dictionary<int, float> getBestScores() {
    if (DataKeeper.difficultyToString() == "normal") {
      return dataInstance.bestNormalScores;
    } else if (DataKeeper.difficultyToString() == "hard") {
      return dataInstance.bestHardScores;
    } else {
      return dataInstance.bestInsaneScores;
    }
  }
  public static int getMaxLevelReached() {
    if (DataKeeper.difficultyToString() == "normal") {
      return dataInstance.maxNormalReached;
    } else if (DataKeeper.difficultyToString() == "hard") {
      return dataInstance.maxHardReached;
    } else {
      return dataInstance.maxInsaneReached;
    }
  }
}
