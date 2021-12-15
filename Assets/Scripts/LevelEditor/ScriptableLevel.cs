using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ScriptableLevel : ScriptableObject {

    public int levelId;
    public int nbRows;
    public int nbColumns;
    public Color topLeftColor;
    public Color topRightColor;
    public Color bottomLeftColor;
    public Color bottomRightColor;
    public List<Vector2Int> moves;
    public List<ScriptableTilePattern> fixedTiles;
}
