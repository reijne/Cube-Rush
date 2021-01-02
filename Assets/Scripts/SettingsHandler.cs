using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsHandler : MonoBehaviour
{
  public PlayerMovement pm;
  public Slider soundSlider;
  public Slider musicSlider;
  public Dropdown songDrop;
  public Dropdown endSongDrop;
  public AudioSource musicSource;
  public AudioSource soundEffectSource;
  public GameObject settingsMenu;
  public static bool settingsActive = false;
  private AudioPlayer audioplayer;
  private bool musicChange = false;
  private int songValue;
  private bool songValueSet = false;
  public bool forceRestart = false;

  private Dictionary<string, AudioClip> songMap = new Dictionary<string, AudioClip>();
  
  private void Awake() {
    GameObject play = GameObject.Find("AudioPlayer");
    musicSource = play.GetComponent<AudioSource>();
    audioplayer = play.GetComponent<AudioPlayer>();
    soundEffectSource = GameObject.Find("DataKeeper").GetComponent<AudioSource>();
  }
  void Start() {
    settingsActive = false;
  }

  public void addSongs() {
    songDrop.options = new List<Dropdown.OptionData>();
    endSongDrop.options = new List<Dropdown.OptionData>();
    songDrop.options.Add(new Dropdown.OptionData("Infinite Silence"));
    endSongDrop.options.Add(new Dropdown.OptionData("Infinite Silence"));
    songDrop.options.Add(new Dropdown.OptionData("Random"));
    endSongDrop.options.Add(new Dropdown.OptionData("Random"));
    foreach(AudioClip song in audioplayer.songs) {
      string songName = song.ToString().Split('(')[0].Trim();
      songMap[songName] = song;
      songDrop.options.Add(new Dropdown.OptionData(songName));
      endSongDrop.options.Add(new Dropdown.OptionData(songName));
    }

    if (DataKeeper.dataInstance.selectedMusic == "Random") {
      playRandomSong();
    } else if (DataKeeper.dataInstance.selectedMusic == "Infinite Silence") {
      musicSource.Stop();
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
    musicSource.loop = false;
    songValue = songDrop.value;
    songValueSet = true;
    forceRestart = false;
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
    endSongDrop.value = 1;
    musicSource.Play();
  }

  private void playSelectedSong() {
    for(int i = 0; i < audioplayer.songs.Count; i++) {
      if (audioplayer.songs[i].ToString() == DataKeeper.dataInstance.selectedMusic) {
        musicSource.clip = audioplayer.songs[i];
        musicSource.Play();
        songDrop.value = i + 2;
            endSongDrop.value = i + 2;
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
      endSongDrop.value = change.value;
      musicSource.Play();
    }
    if (change.value != songValue && songValueSet) {
      musicChange = true;
      songValue = change.value;
      if(forceRestart) {
        newMusicRestart();
      }
    }
  }

  private void newMusicRestart() {
    if (musicChange) {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      musicChange = false;
    } else if (!pm.alive) {
      pm.pc.deathScreen.SetActive(true);
    }
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
      toggleSettingsMenu(false);
    }
  }

  public void toggleSettingsMenu(bool fin) {
    if (pm.pc.fin) return;
    if (pm.pc.deathScreen.activeSelf) pm.pc.deathScreen.SetActive(false);
    settingsMenu.SetActive(!settingsMenu.activeSelf);
    settingsActive = settingsMenu.activeSelf; 
    if (settingsMenu.activeSelf) {
      Debug.Log(musicSlider);
      Debug.Log(DataKeeper.dataInstance.musicVolume);
      musicSlider.value = DataKeeper.dataInstance.musicVolume;
      soundSlider.value = DataKeeper.dataInstance.soundVolume;
    } else {
      if (!fin) {
        newMusicRestart();
      }
    }
  }
  
  public void changeSoundVolume() {
    DataKeeper.dataInstance.soundVolume = soundSlider.value;
    soundEffectSource.volume = soundSlider.value;
    DataKeeper.save();
  }

  public void changeMusicVolume() {
    DataKeeper.dataInstance.musicVolume = musicSlider.value;
    musicSource.volume = musicSlider.value;
    DataKeeper.save();
  }
}
