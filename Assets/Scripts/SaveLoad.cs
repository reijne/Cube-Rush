using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoad
{
    public static void save(Data data) {
      BinaryFormatter bf = new BinaryFormatter();
      string path = Application.persistentDataPath + "/cubeRush.data";

      FileStream stream = new FileStream(path, FileMode.Create);
      bf.Serialize(stream, data);
      stream.Close();
    }

    public static Data load() {
      string path = Application.persistentDataPath + "/cubeRush.data";
      if (File.Exists(path)) {
        Debug.Log(path);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        Data save = bf.Deserialize(stream) as Data;
        stream.Close();
        return save;
      } else {
        Debug.LogWarning("No save found, using standard configurations");
        return null;
      }
    }
}
