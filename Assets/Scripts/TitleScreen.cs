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
  public Text difficultyShower;
  public Slider soundSlider;
  public Slider musicSlider;
  private AudioSource musicSource;
  private AudioSource soundEffectSource;
  private AudioPlayer audiop;
  private bool inSettings = false;

  private void Start() {
    GameObject audioPlayerObj = GameObject.Find("AudioPlayer");
    musicSource = audioPlayerObj.GetComponent<AudioSource>();
    audiop = audioPlayerObj.GetComponent<AudioPlayer>();
    Debug.Log("music currently playin: " + musicSource.isPlaying);
    if (!musicSource.isPlaying) {
      Debug.Log("Setting the clip and starting the music");
      musicSource.clip = audiop.songs[audiop.songs.Count-1];
      musicSource.Play();
    }
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
    if (bestScores.Count == 0) {
      setupTiles();
      return;  
    }
    for(int i = 0; i < frontPlanes.Count; i++) {
      frontPlanes[i].SetActive(true);
      bestScoreTexts[i].text = "";
    }
    bestScoreTexts[frontPlanes.Count].text = "";

    for(int i = 0; i < Mathf.Min(maxLevelReached+1, frontPlanes.Count); i++) {
      frontPlanes[i].SetActive(false);
      bestScoreTexts[i].text = bestScores[i].ToString();
      Debug.Log("level:" + i + "score" + bestScores[i].ToString());
    }
    if (maxLevelReached >= frontPlanes.Count) bestScoreTexts[frontPlanes.Count].text = bestScores[frontPlanes.Count].ToString();
  }

  public void gotoTitleSettings() {
    TitleButtons.SetActive(false);
    TitleSettings.SetActive(true);
    inSettings = true;

    soundSlider.value = DataKeeper.dataInstance.soundVolume;
    musicSlider.value = DataKeeper.dataInstance.musicVolume;

    highlightDifficulty();
  }

  public void gotoTitleScreen() {
    ChallengesScreen.SetActive(false);
    TitleSettings.SetActive(false);
    TitleScreenObj.SetActive(true);
    TitleButtons.SetActive(true);
    inSettings = false;
  }

  public void gotoTitleChallenges() {
    setupTiles();
    TitleScreenObj.SetActive(false);
    ChallengesScreen.SetActive(true);
    difficultyShower.text = DataKeeper.difficultyToString();
    inSettings = false;
  }

  public void quitGame() {
    DataKeeper.save();
    Application.Quit();
  }

  public void loadLevel(int level) {
    if (level <= (DataKeeper.getMaxLevelReached()+1) || level==5) StartCoroutine(DataKeeper.loadAsyncy(level.ToString()));
    // if (level <= (DataKeeper.getMaxLevelReached()+1) || level==5) SceneManager.LoadScene(level.ToString());
  }

  public void setDifficulty(float difficulty) {
    DataKeeper.dataInstance.difficulty = difficulty;
    DataKeeper.save();
  }

  public void changeSoundVolume() {
    DataKeeper.dataInstance.soundVolume = soundSlider.value;
    DataKeeper.save();
  }

  public void changeMusicVolume() {
    DataKeeper.dataInstance.musicVolume = musicSlider.value;
    musicSource.volume = musicSlider.value;
    DataKeeper.save();
  }

  private void FixedUpdate() {
    if (inSettings) {
      highlightDifficulty();    
    }
  }    

  private void highlightDifficulty() {
    Button difficlety;
    if (DataKeeper.difficultyToString() == "easy") {
      difficlety = GameObject.Find("EasyButton").GetComponent<Button>();
    } else if (DataKeeper.difficultyToString() == "normal") {
      difficlety = GameObject.Find("NormalButton").GetComponent<Button>();
    } else if (DataKeeper.difficultyToString() == "hard") {
      difficlety = GameObject.Find("HardButton").GetComponent<Button>();
    } else {
      difficlety = GameObject.Find("InsaneButton").GetComponent<Button>();
    }
    difficlety.Select();
  }
}
