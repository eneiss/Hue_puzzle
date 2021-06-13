using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo
{
    public const int MaxHeight = 20;
    public const int MaxWidth = 20;
    public const int MaxSidePanelWidth = 100;
    public const int MaxGridPanelWidth = 300;
    public const int InitDimensions = 7;
}

[System.Serializable]
public class LevelEditorManager
{
    // public
    public Color[] tiles;
    public int width;
    public int height;
    public int id = 0;
    public List<ScriptableTilePattern> patterns;
    public bool[] tilesInversion;

    // private
    private Color[] corners;

    // computed properties (wrappers for corners[])
    /* REFERENCE:
        level.topLeftColor = this.corners[1];
        level.topRightColor = this.corners[2];
        level.bottomLeftColor = this.corners[0];
        level.bottomRightColor = this.corners[3];
     */
    public Color BottomLeft
    {
        get { return this.corners[0]; }
        set { this.corners[0] = value; }
    }

    public Color TopLeft
    {
        get { return this.corners[1]; }
        set { this.corners[1] = value; }
    }
    public Color TopRight
    {
        get { return this.corners[2]; }
        set { this.corners[2] = value; }
    }
    public Color BottomRight
    {
        get { return this.corners[3]; }
        set { this.corners[3] = value; }
    }

    // -------------- methods
    public LevelEditorManager()
    {
        this.InitValues();
        this.RandomizeColors();
    }

    private void InitValues()
    {

        width = height = LevelInfo.InitDimensions;
        corners = new Color[4];
        tiles = new Color[LevelInfo.MaxWidth * LevelInfo.MaxHeight];
        patterns = new List<ScriptableTilePattern>();

        tilesInversion = new bool[LevelInfo.MaxWidth * LevelInfo.MaxHeight];
        for (int i = 0; i < LevelInfo.MaxWidth * LevelInfo.MaxHeight; ++i)
        {
            tilesInversion[i] = false;
        }
    }

    public void RandomizeColors()
    {
        for (int i = 0; i < 4; ++i)
        {
            corners[i] = RandomColor();
        }
    }

    private Color ComputeColor(int w, int h)
    {
        float red = GetBarycenter(width, height, corners[2].r, corners[1].r, corners[0].r, corners[3].r, h, w);
        float green = GetBarycenter(width, height, corners[2].g, corners[1].g, corners[0].g, corners[3].g, h, w);
        float blue = GetBarycenter(width, height, corners[2].b, corners[1].b, corners[0].b, corners[3].b, h, w);

        return new Color(red, green, blue, 1f);
    }

    private bool TileIsLocked(int w, int h)
    {
        foreach (ScriptableTilePattern pattern in patterns)
        {
            foreach (Tuple<int, int> tuple in pattern)
            {
                if (tuple.Item1 == h && tuple.Item2 == w)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int TileIndex(int w, int h)
    {
        return (LevelInfo.MaxWidth * h) + w;
    }


    // call for each rbg component and get the final value for the component at pos r, c
    private float GetBarycenter(float width, float height, float c1, float c2, float c3, float c4, int r, int c)
    {
        float x1 = c;
        float x2 = width - c - 1f;
        float y1 = r;
        float y2 = height - r - 1f;

        // todo optimize by factorizing
        float c12 = (x1 * c1 + x2 * c2) / (x1 + x2);
        float c43 = (x1 * c4 + x2 * c3) / (x1 + x2);
        float c14 = (y2 * c1 + y1 * c4) / (y1 + y2);
        float c23 = (y2 * c2 + y1 * c3) / (y1 + y2);
        float cVertic = (y2 * c12 + y1 * c43) / (y1 + y2);
        float cHoriz = (x2 * c23 + x1 * c14) / (x1 + x2);

        return (cHoriz + cVertic) / 2;
    }

    private Color RandomColor()
    {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
    }

}
