using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableLevel : ScriptableObject
{

    public int levelId;
    public int nbRows;
    public int nbColumns;
    public Color topLeftColor;
    public Color topRightColor;
    public Color bottomLeftColor;
    public Color bottomRightColor;
    public List<Tuple<int, int>> moves;
    public List<ScriptableTilePattern> fixedTiles;

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
