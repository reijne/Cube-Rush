using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
  public float version;
  public int maxEasyReached;
  public int maxNormalReached;
  public int maxHardReached;
  public int maxInsaneReached;
  public Dictionary<int, float> bestEasyScores;
  public Dictionary<int, float> bestNormalScores;
  public Dictionary<int, float> bestHardScores;
  public Dictionary<int, float> bestInsaneScores;
  public string selectedMusic;
  public float difficulty;
  public float soundVolume;
  public float musicVolume;

  public Data(float version) {
    this.version = version;
    maxEasyReached = -1;
    maxNormalReached = -1;
    maxHardReached = -1;
    maxInsaneReached = -1;

    bestEasyScores = addZeros(new Dictionary<int, float>());
    bestNormalScores = addZeros(new Dictionary<int, float>());
    bestHardScores = addZeros(new Dictionary<int, float>());
    bestInsaneScores = addZeros(new Dictionary<int, float>());

    selectedMusic = "notset";
    difficulty = 4f;
    soundVolume = 0.7f;
    musicVolume = 0.6f;
  }

  private Dictionary<int, float> addZeros(Dictionary<int, float> bestScores) {
    bestScores.Add(0, 0f);
    bestScores.Add(1, 0f);
    bestScores.Add(2, 0f);
    bestScores.Add(3, 0f);
    bestScores.Add(4, 0f);
    bestScores.Add(5, 0f);
    return bestScores;
  }
}
