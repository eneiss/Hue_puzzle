using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{

    public void LoadLevel()
    {
        Debug.Log("Starting level");
        LevelData levelData = (LevelData) gameObject.GetComponent(typeof(LevelData));
        GameObject levelDataObject = GameObject.FindWithTag("LevelData");
        DontDestroyOnLoad(levelDataObject);

        copyLevelData(levelData, (LevelData) levelDataObject.GetComponent(typeof(LevelData)));

        SceneManager.LoadScene("Level");
    }

    // WARNING must be edited when making changes to the level data structure
    void copyLevelData(LevelData source, LevelData dest)
    {
        dest.nbRows = source.nbRows;
        dest.nbColumns = source.nbColumns;
        dest.topLeftColor = source.topLeftColor;
        dest.topRightColor = source.topRightColor;
        dest.bottomRightColor = source.bottomRightColor;
        dest.bottomLeftColor = source.bottomLeftColor;

    }
}
