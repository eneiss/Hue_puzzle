using UnityEngine;

[CreateAssetMenu]
public class ScriptableLevel : ScriptableObject
{

    public int levelId;
    public Color tlColor;

    private void OnEnable()
    {
        Debug.Log("ScriptableLevel loaded");
    }

    private void OnDisable()
    {
        Debug.Log("ScriptableLevel disabled");
    }

    private void OnDestroy()
    {
        Debug.Log("ScriptableLevel destroyed");
    }

}
