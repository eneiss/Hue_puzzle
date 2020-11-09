using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartLevel : MonoBehaviour
{

    public void LoadLevel()
    {
        Debug.Log("Starting level");
        LevelData tempLevelData = (LevelData) gameObject.GetComponent(typeof(LevelData));
        GameObject levelDataObject = GameObject.FindWithTag("LevelData");
        LevelData keepLevelData = (LevelData)levelDataObject.GetComponent(typeof(LevelData));
        DontDestroyOnLoad(levelDataObject);

        keepLevelData.Copy(tempLevelData);

        SceneManager.LoadScene("Level");
    }

}
