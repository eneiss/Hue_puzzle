using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// TODO: check if colors are correctly computed when the size of the grid changes

public class LevelInfo {
    public const int MaxHeight = 20;
    public const int MaxWidth = 20;
    public const int MaxSidePanelWidth = 100;
    public const int MaxGridPanelWidth = 300;
    public const int InitDimensions = 7;
}

[System.Serializable]
public class LevelEditorManager {
    // public
    [NonSerialized]
    public Color[] tiles;       // TODO: use computed/observable properties ?
    public int id = 0;
    [SerializeField]
    public List<ScriptableTilePattern> patterns;
    public bool[] tilesInversion;
    public List<Vector2Int> moves;

    // private
    [SerializeField]
    private Color[] corners;
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    private int currentLevelId;

    private List<(int dx, int dy)> neighbours = new List<(int dx, int dy)>
        {
            (-1, -1), (-1, 1), (1, -1), (1, 1),
            (0, 1), (0, -1), (1, 0), (-1, 0)
        };

    // computed properties (wrappers for corners[])
    /* REFERENCE:
        level.topLeftColor = this.corners[1];
        level.topRightColor = this.corners[2];
        level.bottomLeftColor = this.corners[0];
        level.bottomRightColor = this.corners[3];
     */
    public Color BottomLeft {
        get { return this.corners[0]; }
        set {
            this.corners[0] = value;
            ComputeTileColors();
        }
    }
    public Color TopLeft {
        get { return this.corners[1]; }
        set {
            this.corners[1] = value;
            ComputeTileColors();
        }
    }
    public Color TopRight {
        get { return this.corners[2]; }
        set {
            this.corners[2] = value;
            ComputeTileColors();
        }
    }
    public Color BottomRight {
        get { return this.corners[3]; }
        set {
            this.corners[3] = value;
            ComputeTileColors();
        }
    }

    // public computed width & height (check for bounds)
    public int Width {
        get { return this.width; }
        set {
            if (value < 1)
                this.width = 1;
            else if (value > LevelInfo.MaxWidth)
                this.width = LevelInfo.MaxWidth;
            else
                this.width = value;
        }
    }

    public int Height {
        get { return this.height; }
        set {
            if (value < 1)
                this.height = 1;
            else if (value > LevelInfo.MaxHeight)
                this.height = LevelInfo.MaxHeight;
            else
                this.height = value;
        }
    }

    // -------------- methods
    public LevelEditorManager() {
        this.InitValues();
        this.RandomizeColors();
    }

    private void InitValues() {

        width = height = LevelInfo.InitDimensions;
        corners = new Color[4];
        tiles = new Color[LevelInfo.MaxWidth * LevelInfo.MaxHeight];
        patterns = new List<ScriptableTilePattern>();

        tilesInversion = new bool[LevelInfo.MaxWidth * LevelInfo.MaxHeight];
        for (int i = 0; i < LevelInfo.MaxWidth * LevelInfo.MaxHeight; ++i) {
            tilesInversion[i] = false;
        }

        // determine level id
        string levelsDirPath = Application.dataPath + "/Levels";
        DirectoryInfo levelsDirInfo = new DirectoryInfo(levelsDirPath);
        currentLevelId = (int)levelsDirInfo.CountFilesOfType("asset");

        moves = new List<Vector2Int>();
    }

    public void RemovePatternAt(int i) {
        this.patterns.RemoveAt(i);
    }

    public void RandomizeColors() {
        for (int i = 0; i < 4; ++i) {
            corners[i] = this.RandomColor();
        }

        ComputeTileColors();
    }

    // call this whenever the size, color or inversion of the board has changed
    public void ComputeTileColors() {
        for (int h = 0; h < height; ++h) {
            for (int w = 0; w < width; ++w) {
                tiles[TileIndex(w, h)] = ComputeColor(w, h);
            }
        }
    }

    public void InvertTilesAround(int w, int h) {
        int index;
        if (!TileIsLocked(w, h)) {
            foreach ((int dw, int dh) in neighbours) {
                index = TileIndex(w + dw, h + dh);
                if (!TileIsLocked(w + dw, h + dh) && index >= 0) {      // index = -1 <=> OOB
                    tilesInversion[index] = !tilesInversion[index];
                };
            }

            tilesInversion[TileIndex(w, h)] = !tilesInversion[TileIndex(w, h)];

            moves.Add(new Vector2Int(w, h));

        }
        else {
            Debug.Log("Cant invert locked tile");
        }
    }

    private Color ComputeColor(int w, int h) {
        float red = GetBarycenter(width, height, corners[2].r, corners[1].r, corners[0].r, corners[3].r, h, w);
        float green = GetBarycenter(width, height, corners[2].g, corners[1].g, corners[0].g, corners[3].g, h, w);
        float blue = GetBarycenter(width, height, corners[2].b, corners[1].b, corners[0].b, corners[3].b, h, w);

        return new Color(red, green, blue, 1f);
    }

    public bool TileIsLocked(int w, int h) {
        foreach (ScriptableTilePattern pattern in patterns) {
            foreach (Tuple<int, int> tuple in pattern) {
                if (tuple.Item1 == h && tuple.Item2 == w) {
                    return true;
                }
            }
        }

        return false;
    }

    // OOB check : -1 if OOB
    public int TileIndex(int w, int h) {
        if (w < 0 || w >= width || h < 0 || h >= height)
            return -1;
        return (LevelInfo.MaxWidth * h) + w;
    }

    // call for each rbg component and get the final value for the component at pos r, c
    private float GetBarycenter(float width, float height, float c1, float c2, float c3, float c4, int r, int c) {
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

    private Color RandomColor() {
        return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
    }

    public void SaveLevel(string path) {

        path = path.Replace(Application.dataPath, "Assets");        // use the relative path?

        ScriptableLevel level = ScriptableObject.CreateInstance<ScriptableLevel>();
        AssetDatabase.CreateAsset(level, path);

        level.levelId = this.currentLevelId;
        level.topLeftColor = this.corners[1];
        level.topRightColor = this.corners[2];
        level.bottomLeftColor = this.corners[0];
        level.bottomRightColor = this.corners[3];

        // fixed patterns
        level.fixedTiles = new List<ScriptableTilePattern>();
        int patternNumber = 0;
        foreach (ScriptableTilePattern pattern in this.patterns) {
            pattern.name = "Pattern " + patternNumber;
            level.fixedTiles.Add(pattern);
            AssetDatabase.AddObjectToAsset(pattern, path);
            patternNumber++;
        }

        // moves
        level.moves = new List<Vector2Int>();
        // only add the necessary number of the same move
        var groupedMoves = this.moves.GroupBy(i => i);
        foreach (var group in groupedMoves) {
            if (group.Count() % 2 == 1) {
                level.moves.Add(group.Key);
            }
        }

        // save
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        this.currentLevelId++;
    }

}
