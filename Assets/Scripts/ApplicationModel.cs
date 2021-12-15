using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModel {

    // TODO delete
    public static int levelToLoad = 0;
    // TODO delete
    public static string LevelToLoadPath { get => "Levels/" + levelFileNames [levelToLoad]; }
    // TODO delete
    public static List<string> levelFileNames = new List<string>();

    public static ScriptableLevel scriptableLevel;

}
