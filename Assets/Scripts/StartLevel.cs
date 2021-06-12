using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// FIXME: haptic feedback not working :(

public class StartLevel : MonoBehaviour
{

    //Cache the Manager for performance
    private static HapticFeedbackManager mHapticFeedbackManager;

    public void LoadLevel()
    {
        Debug.Log("Starting level");

        Handheld.Vibrate();
        LevelData tempLevelData = (LevelData) gameObject.GetComponent(typeof(LevelData));
        GameObject levelDataObject = GameObject.FindWithTag("LevelData");
        LevelData keepLevelData = (LevelData)levelDataObject.GetComponent(typeof(LevelData));
        DontDestroyOnLoad(levelDataObject);

        keepLevelData.Copy(tempLevelData);

        SceneManager.LoadScene("Level");
    }


    public static bool HapticFeedback()
    {
        if (mHapticFeedbackManager == null)
        {
            mHapticFeedbackManager = new HapticFeedbackManager();
        }
        return mHapticFeedbackManager.Execute();
    }
}


public class HapticFeedbackManager
{
#if UNITY_ANDROID && !UNITY_EDITOR
        private int HapticFeedbackConstantsKey;
        private AndroidJavaObject UnityPlayer;
#endif

    public HapticFeedbackManager()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            HapticFeedbackConstantsKey=new AndroidJavaClass("android.view.HapticFeedbackConstants").GetStatic<int>("VIRTUAL_KEY");
            UnityPlayer=new AndroidJavaClass ("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer");
            //Alternative way to get the UnityPlayer:
            //int content=new AndroidJavaClass("android.R$id").GetStatic<int>("content");
            //new AndroidJavaClass ("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("findViewById",content).Call<AndroidJavaObject>("getChildAt",0);
#endif
    }

    public bool Execute()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            return UnityPlayer.Call<bool> ("performHapticFeedback",HapticFeedbackConstantsKey);
#endif
        return false;
    }
}
