using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataKeeper : MonoBehaviour
{
  public static Data dataInstance;
  public static bool android = false;
  private static float version = 0.856f;
  /// <summary> Get a load from file, or create a new dataInstance to store all needed information </summary> ///
  private void Awake() {
    if (dataInstance == null) {
      Data loaded = load();
      if (loaded != null) {
        dataInstance = loaded;
      } else {
        dataInstance = new Data(version);
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
      if (loaded.version != version) return null;
      dataInstance = loaded;
    }
    return loaded;
  }

  public static void save() {
    if (dataInstance != null) {
      SaveLoad.save(dataInstance);
    } else {
      Debug.LogWarning("Should never get here, awakening the datakeeper has failed somehow");
      dataInstance = new Data(version);
      SaveLoad.save(dataInstance);
    }
  }

  /// <summary>Return a textual representation of the current difficulty </summary>///
  public static string difficultyToString() {
    if (dataInstance == null) return "";
    if (dataInstance.difficulty >= 3) return "easy";
    else if (dataInstance.difficulty >= 2) return "normal";
    else if (dataInstance.difficulty >= 1) return "hard";
    else return "insane";
  }

  /// <summary>Get the best gotten scores for the current difficulty </summary>///
  public static Dictionary<int, float> getBestScores() {
    if (DataKeeper.difficultyToString() == "easy") {
      return dataInstance.bestEasyScores;
    } else if (DataKeeper.difficultyToString() == "normal") {
      return dataInstance.bestNormalScores;
    } else if (DataKeeper.difficultyToString() == "hard") {
      return dataInstance.bestHardScores;
    } else {
      return dataInstance.bestInsaneScores;
    }
  }

  /// <summary>Get the highest level achieved for the current difficulty </summary>///
  public static int getMaxLevelReached() {
    if (DataKeeper.difficultyToString() == "easy") {
      return dataInstance.maxEasyReached;
    } else if (DataKeeper.difficultyToString() == "normal") {
      return dataInstance.maxNormalReached;
    } else if (DataKeeper.difficultyToString() == "hard") {
      return dataInstance.maxHardReached;
    } else {
      return dataInstance.maxInsaneReached;
    }
  }

  public static IEnumerator loadAsyncy(string level) {
    AsyncOperation op = SceneManager.LoadSceneAsync(level);
    while (!op.isDone) {
      Debug.Log("loading");
      yield return null;
    }
  }
}
