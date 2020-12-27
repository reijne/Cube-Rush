using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsHandler : MonoBehaviour
{
  public Slider soundSlider;
  public Slider musicSlider;
  public Dropdown songDrop;
  public AudioSource musicSource;
  public GameObject settingsMenu;
  private AudioPlayer audioplayer;
  private int cooldown = 0;
  private bool musicChange = false;
  private int songValue;
  private bool songValueSet = false;

  private Dictionary<string, AudioClip> songMap = new Dictionary<string, AudioClip>();
  
  private void Awake() {
    GameObject play = GameObject.Find("AudioPlayer");
    musicSource = play.GetComponent<AudioSource>();
    audioplayer = play.GetComponent<AudioPlayer>();
  }
  void Start() {
    songDrop.options = new List<Dropdown.OptionData>();
    songDrop.options.Add(new Dropdown.OptionData("Infinite Silence"));
    songDrop.options.Add(new Dropdown.OptionData("Random"));
    foreach(AudioClip song in audioplayer.songs) {
      string songName = song.ToString().Split('(')[0];
      songMap[songName] = song;
      songDrop.options.Add(new Dropdown.OptionData(songName));
    }

    if (DataKeeper.dataInstance.selectedMusic == "Random") {
      playRandomSong();
    } else if (DataKeeper.dataInstance.selectedMusic == "Infinite Silence") {
      return;
    } else {
      if (DataKeeper.dataInstance.selectedMusic == "notset") {
        if (audioplayer.songs.Count > 0) {
          DataKeeper.dataInstance.selectedMusic = audioplayer.songs[0].ToString(); 
        } else {
          DataKeeper.dataInstance.selectedMusic = "Random";
        }
      }
      playSelectedSong();
    }
    songValue = songDrop.value;
    songValueSet = true;
  }

  public void gotoTitleScreen() {
    SceneManager.LoadScene("title");
    musicSource.Stop();
  }

  private void playRandomSong() {
    if (audioplayer.songs.Count == 0) return;
    int songIndex = Random.Range(0, audioplayer.songs.Count);
    musicSource.clip = audioplayer.songs[songIndex];
    songDrop.value = 1;
    musicSource.Play();
  }

  private void playSelectedSong() {
    for(int i = 0; i < audioplayer.songs.Count; i++) {
      if (audioplayer.songs[i].ToString() == DataKeeper.dataInstance.selectedMusic) {
        musicSource.clip = audioplayer.songs[i];
        musicSource.Play();
        songDrop.value = i + 2;
        return;
      }
    }
  }

  public void selectSong(Dropdown change) {
    if (change.value == 0) {
      DataKeeper.dataInstance.selectedMusic = "Infinite Silence";
      musicSource.Stop();
    } else if (change.value == 1) {
      DataKeeper.dataInstance.selectedMusic = "Random";
    } else {
      musicSource.clip = audioplayer.songs[change.value-2];
      DataKeeper.dataInstance.selectedMusic = audioplayer.songs[change.value-2].ToString();
      songDrop.value = change.value;
      musicSource.Play();
    }
    if (change.value != songValue && songValueSet) {
      musicChange = true;
      songValue = change.value;
      Debug.Log("Value has changered");
    }
  }

  public void backPressed() {
    settingsMenu.SetActive(false);
    newMusicRestart();
  } 

  private void newMusicRestart() {
    if (musicChange) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      musicChange = false;
    }
  }

  private void FixedUpdate() {
    if (Input.GetKey(KeyCode.Escape) && cooldown == 0) {
      settingsMenu.SetActive(!settingsMenu.activeSelf);
      if (settingsMenu.activeSelf) {
        soundSlider.value = DataKeeper.dataInstance.musicVolume;
      } else {
        newMusicRestart();
      }
      cooldown = 50;
    }
    if (cooldown > 0) cooldown--;
  }
  
  public void changeVolume() {
    DataKeeper.dataInstance.musicVolume = soundSlider.value;
    musicSource.volume = soundSlider.value;
    DataKeeper.save();
  }
}
