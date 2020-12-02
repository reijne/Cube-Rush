using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public PlayerMovement pm;
    public GameObject player;
    public GameObject small;
    public GameObject medium;
    public int spawnCount;

    public float ymin_small;
    public float ymin_med;
    public float xmin;
    public float xmax;
    // Start is called before the first frame update
    void Start()
    {
      player = GameObject.Find("Player");
      pm = player.GetComponent<PlayerMovement>();
      StartCoroutine(spawner());
    }

    private void spawnWave() {
      int choice;
      float x, z;
      // Debug.Log("spawn a wave");
      for (int i = 0; i <= spawnCount; i++) {
        x = Random.Range(xmin, xmax);
        z = 2*pm.speed;
        choice = Random.Range(0, 2);
        if (choice == 0) {
          spawnSmall(x, z);
        } else {
          spawnMedium(x, z);
        }
      }
      spawnBeforePlayer();
    }

    private void spawnSmall(float x, float z) {
      GameObject s = Instantiate(small) as GameObject;
      s.transform.position = new Vector3(x, Random.Range(ymin_small, ymin_small+2f), z);
    }

    private void spawnMedium(float x, float z) {
      GameObject m = Instantiate(medium) as GameObject;
      m.transform.position = new Vector3(x, Random.Range(ymin_med, ymin_med+4f), z);
    }

    private void spawnBeforePlayer() {
      GameObject m = Instantiate(medium) as GameObject;
      m.transform.position = pm.rb.transform.position + new Vector3(0, 0, 40+pm.speed);
      if (pm.speed > 75) {
        GameObject o = Instantiate(medium) as GameObject;
        int choice = Random.Range(0, 2);
        int x = 5;
        if (choice == 0) {
          x = -5;
        }
        o.transform.position = pm.rb.transform.position + new Vector3(x, 0, 40+pm.speed);
        if (pm.speed > 130) {
          GameObject p = Instantiate(medium) as GameObject;
          p.transform.position = pm.rb.transform.position + new Vector3(-x, 0, 40+pm.speed);
        }
      }
    }

    IEnumerator spawner() {
      while (pm.alive) {
        yield return new WaitForSeconds(Random.Range(0.3f, 1.4f));
        spawnWave();
      }
    }
}
