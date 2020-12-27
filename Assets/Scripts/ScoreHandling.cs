using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreHandling : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scoreObj;
    public PlayerMovement pm;
    public PlayerCollision pc;
    public Data data;
    public float penalty = 0;
    public float score = 0;
    private bool highscoreHandled = false;
    Dictionary<int, float> currentBestScores;


    // Update is called once per frame
    private void Start() {
      scoreObj.fontSize = 24;
      highscoreHandled = false;
    }
    private void FixedUpdate() {
      if (pm.infinite) {
        infinityScoreHandler();
      } else {
        levelScoreHandler();
      }
    }

    private void infinityScoreHandler() {
      if (penalty <= 0 && pm.alive) {
        score += (Time.time - pm.timeRoundStart) / DataKeeper.dataInstance.difficulty;
      } else {
        penalty--;
      }

      if (pm.alive) {
        showScore();
      } else {
        handleBestScores();
      }
    }

    private void levelScoreHandler() {
      if (!pc.fin) {
        if (pm.horizontalInput != 0) {
          score -= 10 / Mathf.Max(DataKeeper.dataInstance.difficulty, 1);
        }
        showScore();
      } else {
        handleBestScores();
      }
    }

    private void showScore() {
      scoreObj.text = convertScore(score);
    }

    public void handleBestScores() {
      if (highscoreHandled) return;
      score = Mathf.Round(score);
      Data loaded = SaveLoad.load();
      Dictionary<int, float> loadedBestScores;

      if (loaded == null) {
        Debug.Log("Load not found");
        showBestScore(true);
      } else {
        if (DataKeeper.difficultyToString() == "easy") {
          loadedBestScores = loaded.bestEasyScores;
        } else if (DataKeeper.difficultyToString() == "normal") {
          loadedBestScores = loaded.bestNormalScores;
        } else if (DataKeeper.difficultyToString() == "hard") {
          loadedBestScores = loaded.bestHardScores;
        } else {
          loadedBestScores = loaded.bestInsaneScores;
        }

        if (score > loadedBestScores[pm.currentLevel]) {
          Debug.Log("Score better than in savefile");
          showBestScore(true);
          DataKeeper.getBestScores()[pm.currentLevel] = score;
        } else {
          Debug.Log("Score not better than saved file");
          Debug.Log(score);
          Debug.Log(loadedBestScores[pm.currentLevel]);
          showBestScore(false);
        }
      }
      DataKeeper.save();
      highscoreHandled = true;
    }

    private void showBestScore(bool isBestScore) {
      scoreObj.fontSize = 32;
      if (isBestScore) {
        scoreObj.text = "NEW Best: " + convertScore(score);
      } else {
        string current = convertScore(score); 
        Debug.Log(current);

        score = DataKeeper.getBestScores()[pm.currentLevel];
        scoreObj.text = current + "    Best: " + convertScore(score);
      }
    }

    public string convertScore(float localScore) {
      if (localScore > 1000000) {
        return (localScore/1000000).ToString("F2") + "M"; 
      } else if (localScore > 1000) {
        return (localScore/1000).ToString("F1") + "k"; 
      } else {
        return Mathf.Round(localScore).ToString(); 
      }
    }
}
