using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

/*
 * TODO
 * 
 *  - delegate all logic- and data-related stuff to a new Level model to create
 *  in the Model folder, make it serializable and save this object in JSON
 *  instead of saving this editor window as an asset
 *  
 *  - use intrisic data structures to store fixed tiles patterns (to avoid
 *  shallow copy issues) -> use arrays
 *  
 *  - get user click on the window, determine if some non-fixed tile was
 *  clicked and invert it if needed + store the click's position in the grid
 * 
 * */

public class LevelEditorWindow : EditorWindow
{
    private Color[] corners;
    private Color[] tiles;
    private int width;
    private int height;
    private int id = 0;
    private List<ScriptableTilePattern> patterns;
    private bool[] tilesInversion;

    // TODO use LevelInfo in ScriptableLevel (or move it) instead
    const int INIT_DIMENSIONS = 7;

    const int MAX_SIDE_PANEL_WIDTH = 100;
    const int MAX_GRID_PANEL_WIDTH = 300;

    Texture2D lockedTex;
    private Event curEvt;

    [MenuItem("Custom/Level Editor")]
    public static void ShowLevelEditorWindow()
    {
        // find window if existing, else create it
        EditorWindow window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("Level Editor");
    }

    // called whenever the editor window is created
    public void OnEnable()
    {
        width = height = INIT_DIMENSIONS;
        corners = new Color[4];
        tiles = new Color[LevelInfo.MaxWidth * LevelInfo.MaxHeight];
        patterns = new List<ScriptableTilePattern>();

        for (int i = 0; i < 4; ++i)
        {
            corners[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
        }

        tilesInversion = new bool[LevelInfo.MaxWidth * LevelInfo.MaxHeight];
        for (int i = 0; i < LevelInfo.MaxWidth * LevelInfo.MaxHeight; ++i)
        {
            tilesInversion[i] = false;
        }

        SetLockedTexture();
    }

    // call for each rbg component and get the final value for the component at pos r, c
    float GetBarycenter(float c1, float c2, float c3, float c4, int r, int c)
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

    private Color ComputeColor(int w, int h)
    {
        float red = GetBarycenter(corners[2].r, corners[1].r, corners[0].r, corners[3].r, h, w);
        float green = GetBarycenter(corners[2].g, corners[1].g, corners[0].g, corners[3].g, h, w);
        float blue = GetBarycenter(corners[2].b, corners[1].b, corners[0].b, corners[3].b, h, w);

        return new Color(red, green, blue, 1f);
    }

    private int TileIndex(int w, int h)
    {
        return (LevelInfo.MaxWidth * h) + w;
    }

    /*
    private void ApplyColors()
    {

        Debug.Log("Applying new colors");
        for (int w = 0; w < width; ++w)
        {

            for (int h = 0; h < height; ++h)
            {
                tiles[h*width + w] = ComputeColor(w, h);
            }
        }
    } 
    */

    private void SetLockedTexture()
    {
        int size = 10;
        lockedTex = new Texture2D(size, size);

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                if ((i+j)%4 < 2)
                {
                    lockedTex.SetPixel(i, j, new Color(0.9f, 0.9f, 0.9f, 1f));
                } else
                {
                    lockedTex.SetPixel(i, j, new Color(1f, 0f, 0f, 1f));
                }
            }
        }
    }

    private static long DirCountAssets(DirectoryInfo d)
    {
        Debug.Log(d.FullName);
        long i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Extension.Equals(".asset", StringComparison.OrdinalIgnoreCase))
                i++;
        }
        return i;
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

    private void CopyLevelData(ScriptableLevel level)
    {
        if (this.id == 0) 
        {           // default id
            //string currentDir = Environment.CurrentDirectory;
            DirectoryInfo directory = new DirectoryInfo("Assets/Levels");
            this.id = (int) DirCountAssets(directory);      // should not overflow
        }
        level.levelId = this.id;
        level.topLeftColor = this.corners[1];
        level.topRightColor = this.corners[2];
        level.bottomLeftColor = this.corners[0];
        level.bottomRightColor = this.corners[3];

        // fixme shallow copy not working :/
        level.fixedTiles = this.patterns;
    }

    private void SaveLevel(string path)
    {
        path = path.Replace(Application.dataPath, "Assets");
        ScriptableLevel scriptableLevel = ScriptableObject.CreateInstance<ScriptableLevel>();
        Debug.Log(scriptableLevel.levelId);
        CopyLevelData(scriptableLevel);
        AssetDatabase.CreateAsset(scriptableLevel, path);
        AssetDatabase.Refresh();


        string jsonPattern = JsonUtility.ToJson(this.patterns[0]);
        Debug.Log(jsonPattern);

        Debug.Log("File successfully saved at " + path);
    }

    // scriptable tile pattern editor layout
    private void DoPatternAt(int i)
    {
        ScriptableTilePattern pattern = patterns[i];

        GUILayout.BeginHorizontal();
        GUILayout.Label("Pattern " + (i + 1), EditorStyles.label);
        if (GUILayout.Button("(-) Remove Pattern"))
        {
            // todo confirm window
            patterns.RemoveAt(i);
        }
        GUILayout.EndHorizontal();

        // leftMargin
        GUILayout.BeginHorizontal();
        pattern.SetCoffset(EditorGUILayout.IntField("Left Margin", pattern.leftMargin));
        if (GUILayout.Button("-") && pattern.leftMargin > 0)
            pattern.SetCoffset(pattern.leftMargin - 1);
        if (GUILayout.Button("+") && pattern.leftMargin < height - 1)
            pattern.SetCoffset(pattern.leftMargin + 1);
        GUILayout.EndHorizontal();

        // topMargin
        GUILayout.BeginHorizontal();
        pattern.SetRoffset(EditorGUILayout.IntField("Top Margin", pattern.topMargin));
        if (GUILayout.Button("-") && pattern.topMargin > 0)
            pattern.SetRoffset(pattern.topMargin - 1);
        if (GUILayout.Button("+") && pattern.topMargin < width - 1)
            pattern.SetRoffset(pattern.topMargin + 1);
        GUILayout.EndHorizontal();

        // rightMargin
        GUILayout.BeginHorizontal();
        pattern.SetCendoffset(EditorGUILayout.IntField("Right Margin", pattern.rightMargin));
        if (GUILayout.Button("-") && pattern.rightMargin > 0)
            pattern.SetCendoffset(pattern.rightMargin - 1);
        if (GUILayout.Button("+") && height - pattern.rightMargin > pattern.leftMargin + 1)
            pattern.SetCendoffset(pattern.rightMargin + 1);
        GUILayout.EndHorizontal();

        // bottomMargin
        GUILayout.BeginHorizontal();
        pattern.SetRendoffset(EditorGUILayout.IntField("Bottom Margin", pattern.bottomMargin));
        if (GUILayout.Button("-") && pattern.bottomMargin > 0)
            pattern.SetRendoffset(pattern.bottomMargin - 1);
        if (GUILayout.Button("+") && width - pattern.bottomMargin > pattern.topMargin + 1)
            pattern.SetRendoffset(pattern.bottomMargin + 1);
        GUILayout.EndHorizontal();

        // spacing
        GUILayout.BeginHorizontal();
        pattern.SetSpacing(EditorGUILayout.IntField("Vert. Step", pattern.spacing));
        if (GUILayout.Button("-") && pattern.spacing > -1)
            pattern.SetSpacing(pattern.spacing - 1);
        if (GUILayout.Button("+"))
            pattern.SetSpacing(pattern.spacing + 1);
        GUILayout.EndHorizontal();

        // repeat
        GUILayout.BeginHorizontal();
        pattern.SetRepeat(EditorGUILayout.IntField("Horiz. Step", pattern.repeat));
        if (GUILayout.Button("-") && pattern.repeat > -1)
            pattern.SetRepeat(pattern.repeat - 1);
        if (GUILayout.Button("+"))
            pattern.SetRepeat(pattern.repeat + 1);
        GUILayout.EndHorizontal();

    }

    private void DoCanvas()
    {
        Color oldColor = GUI.color;
        GUILayout.BeginVertical();

        for (int h = 0; h < height; ++h)
        {
            GUILayout.BeginHorizontal();

            for (int w = 0; w < width; ++w)
            {
                tiles[TileIndex(w, h)] = ComputeColor(w, h);
                GUI.color = tiles[TileIndex(w, h)];

                Texture2D tex = EditorGUIUtility.whiteTexture;

                // reserve a rect in the GUI layout system
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                // debug
                // Debug.Log("Rect: " + rect.x + ", " + rect.y);
                
                // then use it
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                // draw borders around the tile if it is locked
                if (TileIsLocked(w, h))
                {
                    GUI.DrawTexture(rect, tex, ScaleMode.StretchToFill, alphaBlend: true, imageAspect: 0, color: new Color(0f, 0f, 0f, 1f), 5f, 0f);
                }

            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUI.color = oldColor;
    }

    // side tools panel
    private void DoControls()
    {
        GUILayout.BeginVertical(GUILayout.MaxWidth(MAX_GRID_PANEL_WIDTH));

        GUILayout.Label("Toolbar", EditorStyles.largeLabel);

        // horizontal line separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ----- dimensions of the grid
        GUILayout.Label("Grid dimensions", EditorStyles.label);

        // width
        GUILayout.BeginHorizontal();
        width = EditorGUILayout.IntField("Width", width);
        if (GUILayout.Button("-") && width > 1)
            width -= 1;
        if (GUILayout.Button("+") && width < LevelInfo.MaxWidth)
            width += 1;
        GUILayout.EndHorizontal();

        // height
        GUILayout.BeginHorizontal();
        height = EditorGUILayout.IntField("Height", height);
        if (GUILayout.Button("-") && height > 1)
        {
            height -= 1;
            foreach (ScriptableTilePattern pattern in patterns)
            {
                pattern.SetHeight(height);
            }
        }
        if (GUILayout.Button("+") && width < LevelInfo.MaxHeight)
        {
            height += 1;
            foreach (ScriptableTilePattern pattern in patterns)
            {
                pattern.SetHeight(height);
            }
        }
        GUILayout.EndHorizontal();

        // check for bounds
        if (width < 0)
            width = 0;
        if (width > LevelInfo.MaxWidth)
            width = LevelInfo.MaxWidth;
        if (height < 0)
            height = 0;
        if (height > LevelInfo.MaxHeight)
            height = LevelInfo.MaxHeight;

        // horizontal line separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ----- colors
        GUILayout.Label("Corner colors", EditorStyles.label);

        // colors of the corners
        GUILayout.BeginHorizontal();
        corners[1] = EditorGUILayout.ColorField(corners[1], GUILayout.MaxWidth(MAX_SIDE_PANEL_WIDTH));
        corners[2] = EditorGUILayout.ColorField(corners[2], GUILayout.MaxWidth(MAX_SIDE_PANEL_WIDTH));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        corners[0] = EditorGUILayout.ColorField(corners[0], GUILayout.MaxWidth(MAX_SIDE_PANEL_WIDTH));
        corners[3] = EditorGUILayout.ColorField(corners[3], GUILayout.MaxWidth(MAX_SIDE_PANEL_WIDTH));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // randomize colors button
        if (GUILayout.Button("Randomize Colors")) {
            for (int i = 0; i < 4; ++i)
            {
                corners[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
            }
        }

        // horizontal line separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ----- fixed tiles
        GUILayout.Label("Fixed Tiles Patterns", EditorStyles.label);

        EditorGUILayout.Space();

        // render each pattern
        for (int i = 0; i < patterns.Count; ++i)
        {
            DoPatternAt(i);
        }

        // add new pattern
        if (GUILayout.Button("(+) New Pattern"))
        {
            patterns.Add(ScriptableObject.CreateInstance<ScriptableTilePattern>());
            patterns[patterns.Count - 1].SetHeight(height);
            patterns[patterns.Count - 1].SetWidth(width);
        }

        // save button
        if (GUILayout.Button("Save as..."))
        {
            string path = EditorUtility.SaveFilePanel("Save level as", "Assets/Levels", "level.asset", "asset");

            if (path.Length !=0)
            {
                SaveLevel(path);
            }
        }

        // TODO level id selector

        GUILayout.EndVertical();
    }

    // event handler from https://answers.unity.com/questions/62859/left-click-up-event-in-editor.html
    void checkForRelease()
    {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        switch (e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                GUIUtility.hotControl = controlID;
                e.Use();
                break;
            case EventType.MouseUp:
                GUIUtility.hotControl = 0;
                e.Use();
                Debug.Log("up !");
                break;
            case EventType.MouseDrag:
                GUIUtility.hotControl = controlID;
                e.Use();
                break;
        }
    }

    void HandleMouseUp(Event evt)
    {
        // always 300
        //Debug.Log(GUILayoutUtility.GetLastRect().x + GUILayoutUtility.GetLastRect().width);
        // Debug.Log("Handling mouse up on editor window at position " + evt.mousePosition.x + ", " + evt.mousePosition.y);
        // Debug.Log("Window width: " + position.width + ", height: " + position.height);

        // TODO: fix the x that gets rounded to 0 when when -1 < x < 0 and round it to -1 instead
        int x = (int) ( ((int)evt.mousePosition.x - MAX_GRID_PANEL_WIDTH) * width / (position.width - MAX_GRID_PANEL_WIDTH) );
        int y = (int) ( (int)evt.mousePosition.y * height / position.height);
        Debug.Log("Click at " + x + ", " + y);
    }

    // called once per frame -> no need to explicitely call ApplyColors() ?
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        DoControls();


        curEvt = Event.current;
        // Debug.Log(curEvt.type);
        if(curEvt.type == EventType.MouseUp)
        {
            HandleMouseUp(curEvt);
        }

        DoCanvas();
        GUILayout.EndHorizontal();
    }
}
