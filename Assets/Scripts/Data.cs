using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
  public int maxNormalReached;
  public int maxHardReached;
  public int maxInsaneReached;
  public Dictionary<int, float> bestNormalScores;
  public Dictionary<int, float> bestHardScores;
  public Dictionary<int, float> bestInsaneScores;
  public string selectedMusic;
  public float difficulty;
  public float volume;

  public Data() {
    maxNormalReached = -1;
    maxHardReached = -1;
    maxInsaneReached = -1;

    bestNormalScores = addZeros(new Dictionary<int, float>());
    bestHardScores = addZeros(new Dictionary<int, float>());
    bestInsaneScores = addZeros(new Dictionary<int, float>());

    selectedMusic = "notset";
    difficulty = 2;
    volume = 0.7f;
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
