using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionManager : MonoBehaviour {

    public ScriptableLevel[] levels;

    void Start() {

        foreach (ScriptableLevel level in levels) {
            ApplicationModel.loadedLevels.Add(level);
        }
    }

    public void OnLevelChoice(int levelNo) {

        ApplicationModel.levelToLoad = levelNo - 1;

        SceneManager.LoadScene("Level");
    }
}
