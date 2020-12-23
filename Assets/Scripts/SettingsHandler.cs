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
  public AudioSource audios;
  public GameObject settingsMenu;
  private AudioPlayer audioplayer;
  private int cooldown = 0;

  private Dictionary<string, AudioClip> songMap = new Dictionary<string, AudioClip>();
  
  private void Awake() {
    GameObject play = GameObject.Find("AudioPlayer");
    audios = play.GetComponent<AudioSource>();
    audioplayer = play.GetComponent<AudioPlayer>();
  }
  void Start()
  {
    songDrop.options = new List<Dropdown.OptionData>();
    songDrop.options.Add(new Dropdown.OptionData("none"));
    songDrop.options.Add(new Dropdown.OptionData("random"));
    foreach(AudioClip song in audioplayer.songs) {
      string songName = song.ToString().Split('(')[0];
      songMap[songName] = song;
      songDrop.options.Add(new Dropdown.OptionData(songName));
    }

    if (DataKeeper.dataInstance.selectedMusic == "random") {
      playRandomSong();
    } else if (DataKeeper.dataInstance.selectedMusic == "none") {
      return;
    } else {
      if (DataKeeper.dataInstance.selectedMusic == "notset") {
        if (audioplayer.songs.Count > 0) {
          DataKeeper.dataInstance.selectedMusic = audioplayer.songs[0].ToString(); 
        } else {
          DataKeeper.dataInstance.selectedMusic = "random";
        }
      }
      playSelectedSong();
    }
  }

  public void gotoTitleScreen() {
    SceneManager.LoadScene("title");
    audios.Stop();
  }

  private void playRandomSong() {
    if (audioplayer.songs.Count == 0) return;
    int songIndex = Random.Range(0, audioplayer.songs.Count);
    audios.clip = audioplayer.songs[songIndex];
    songDrop.value = 1;
    audios.Play();
  }

  private void playSelectedSong() {
    for(int i = 0; i < audioplayer.songs.Count; i++) {
      if (audioplayer.songs[i].ToString() == DataKeeper.dataInstance.selectedMusic) {
        audios.clip = audioplayer.songs[i];
        audios.Play();
        songDrop.value = i + 2;
        return;
      }
    }
  }

  public void selectSong(Dropdown change) {
    if (change.value == 0) {
      DataKeeper.dataInstance.selectedMusic = "none";
      audios.Stop();
    } else if (change.value == 1) {
      DataKeeper.dataInstance.selectedMusic = "random";
    } else {
      audios.clip = audioplayer.songs[change.value-2];
      DataKeeper.dataInstance.selectedMusic = audioplayer.songs[change.value-2].ToString();
      songDrop.value = change.value;
      audios.Play();
    }
  }

  public void backPressed() {
    settingsMenu.SetActive(false);
  } 

  private void FixedUpdate() {
    if (Input.GetKey(KeyCode.Escape) && cooldown == 0) {
      settingsMenu.SetActive(!settingsMenu.activeSelf);
      if (settingsMenu.activeSelf) soundSlider.value = DataKeeper.dataInstance.volume;
      cooldown = 50;
    }
    if (cooldown > 0) cooldown--;
  }
  
  public void changeVolume() {
    DataKeeper.dataInstance.volume = soundSlider.value;
    audios.volume = soundSlider.value;
    DataKeeper.save();
  }
}
