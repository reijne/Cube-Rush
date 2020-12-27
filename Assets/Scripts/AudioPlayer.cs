using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
  // Start is called before the first frame update
  public static AudioPlayer instance;
  public List<AudioClip> songs;
  public List<AudioClip> sounds;
  private void Awake() {
    if (instance == null) {
      instance = this;
      DontDestroyOnLoad(instance);
    } else {
      Destroy(this);
    }
    
    if (DataKeeper.dataInstance != null) {
      instance.GetComponent<AudioSource>().volume = DataKeeper.dataInstance.musicVolume;
    } else {
      instance.GetComponent<AudioSource>().volume = 0.7f;
    }
  }
}
