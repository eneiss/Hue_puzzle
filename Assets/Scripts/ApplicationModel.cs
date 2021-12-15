using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModel {

    // 0-based index
    public static int levelToLoad = -1;

    public static ScriptableLevel ScriptableLevel { get => loadedLevels[levelToLoad]; }
    public static List<ScriptableLevel> loadedLevels = new List<ScriptableLevel>();

}
