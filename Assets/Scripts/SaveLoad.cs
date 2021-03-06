﻿using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class SaveLoad {

    public static void Save(string toSave, string path)
    {
        var encoding = Encoding.GetEncoding("UTF-8");
        using (StreamWriter stream = new StreamWriter(Path.Combine(Application.dataPath, path), false, encoding))
        {
            stream.Write(toSave);
        }
    }

    public static string Load(string path)
    {
        string filePath = path.Replace(".json", "");

        TextAsset targetFile = Resources.Load<TextAsset>(filePath);

        return targetFile.text;
    }
}
