using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public int nbRows;
    public int nbColumns;
    public Color topLeftColor;
    public Color topRightColor;
    public Color bottomLeftColor;
    public Color bottomRightColor;
    public List<string> moves;
    public List<FixedTilePattern> fixedTiles;

    public void Copy(LevelData source)
    {
        this.nbRows = source.nbRows;
        this.nbColumns = source.nbColumns;
        this.topLeftColor = source.topLeftColor;
        this.topRightColor = source.topRightColor;
        this.bottomRightColor = source.bottomRightColor;
        this.bottomLeftColor = source.bottomLeftColor;

        // shallow copy !!!!
        // WARNING must be edited when making changes to the level data structure
        foreach (string move in source.moves)
        {
            this.moves.Add(move);
        }

        foreach (FixedTilePattern pattern in source.fixedTiles)
        {
            FixedTilePattern newPattern = gameObject.AddComponent<FixedTilePattern>();
            // FixedTilePattern newPattern = new FixedTilePattern();
            this.fixedTiles.Add(newPattern);
            newPattern.Copy(pattern);

        }
    }

}
