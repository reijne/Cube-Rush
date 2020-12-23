using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TitleScreen : MonoBehaviour
{
  // public GameObject player;
  public GameObject TitleScreenObj;
  private TitleScreen title;
  public GameObject TitleButtons;
  public GameObject TitleSettings;
  public GameObject ChallengesScreen;
  public List<GameObject> frontPlanes;
  public List<Text> bestScoreTexts;
  public Slider soundSlider;

  private void Start() {
    setupTiles();
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKey(KeyCode.Escape)) {
      gotoTitleScreen();
    }
  }

  private void setupTiles() {

    float maxLevelReached = DataKeeper.getMaxLevelReached();
    Dictionary<int, float> bestScores = DataKeeper.getBestScores();
    for(int i = 0; i < frontPlanes.Count; i++) {
      frontPlanes[i].SetActive(true);
      bestScoreTexts[i].text = "";
    }
    bestScoreTexts[frontPlanes.Count].text = "";

    for(int i = 0; i < Mathf.Min(maxLevelReached+1, frontPlanes.Count); i++) {
      frontPlanes[i].SetActive(false);
      bestScoreTexts[i].text = bestScores[i].ToString();
    }
  }

  public void gotoTitleSettings() {
    TitleButtons.SetActive(false);
    TitleSettings.SetActive(true);

    soundSlider.value = DataKeeper.dataInstance.volume;

    Button difficlety;
    if (DataKeeper.dataInstance.difficulty == 2) {
      difficlety = GameObject.Find("EasyButton").GetComponent<Button>();
    } else if (DataKeeper.dataInstance.difficulty == 1) {
      difficlety = GameObject.Find("NormalButton").GetComponent<Button>();
    } else {
      difficlety = GameObject.Find("HardButton").GetComponent<Button>();
    }
    difficlety.Select();
  }

  public void gotoTitleScreen() {
    ChallengesScreen.SetActive(false);
    TitleSettings.SetActive(false);
    TitleScreenObj.SetActive(true);
    TitleButtons.SetActive(true);
  }

  public void gotoTitleChallenges() {
    setupTiles();
    TitleScreenObj.SetActive(false);
    ChallengesScreen.SetActive(true);
  }

  public void quitGame() {
    DataKeeper.save();
    Application.Quit();
  }

  public void loadLevel(int level) {
    if (level <= (DataKeeper.getMaxLevelReached()+1) || level==5) SceneManager.LoadScene(level.ToString());
  }

  public void setDifficulty(float difficulty) {
    DataKeeper.dataInstance.difficulty = difficulty;
    DataKeeper.save();
  }

  public void changeVolume() {
    DataKeeper.dataInstance.volume = soundSlider.value;
    DataKeeper.save();
  }
}
