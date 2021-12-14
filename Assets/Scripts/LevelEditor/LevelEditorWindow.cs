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

    private LevelEditorManager manager;

    private Texture2D lockedTex;
    private Event curEvt;

    [MenuItem("Level Editor/Editor")]
    public static void ShowLevelEditorWindow()
    {
        // find window if existing, else create it
        EditorWindow window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("Level Editor");
    }

    // called whenever the editor window is created
    public void OnEnable()
    {
        manager = new LevelEditorManager();

        CreateLockedTileTexture();
    }


    // called once per frame
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        DoControls();


        curEvt = Event.current;
        // Debug.Log(curEvt.type);
        if (curEvt.type == EventType.MouseUp)
        {
            HandleMouseUp(curEvt);
        }

        DoCanvas();
        GUILayout.EndHorizontal();
    }

    // create the texture for the border around locked/fixed tiles
    private void CreateLockedTileTexture()
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


    // TODO replace with a JSON serialization of LevelEditorManager -> or not
    // or remove this and do the logic in the manager only
    private void SaveLevel(string path)
    {

        manager.SaveLevel(path);


        //string levelJson = JsonUtility.ToJson(manager);
        //Debug.Log(levelJson);

        //string patternJson = JsonUtility.ToJson(manager.patterns[0]);
        //Debug.Log(patternJson);

        //Debug.Log("File successfully saved at " + path);
    }
    // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    // scriptable tile pattern editor layout
    private void DoPatternAt(int i)
    {
        ScriptableTilePattern pattern = manager.patterns[i];

        GUILayout.BeginHorizontal();
        GUILayout.Label("Pattern " + (i + 1), EditorStyles.label);
        if (GUILayout.Button("(-) Remove Pattern"))
        {
            // todo confirm window
            if (EditorUtility.DisplayDialog(title: "Warning: Removing pattern", 
                message: "Remove fixed tiles pattern " + (i + 1) + " ?\nThis action cannot be undone.", 
                ok: "Yes", cancel: "Noooo !"))
            {
                manager.RemovePatternAt(i);
            }
        }
        GUILayout.EndHorizontal();

        // leftMargin
        GUILayout.BeginHorizontal();
        pattern.SetCoffset(EditorGUILayout.IntField("Left Margin", pattern.leftMargin));
        if (GUILayout.Button("-") && pattern.leftMargin > 0)
            pattern.SetCoffset(pattern.leftMargin - 1);
        if (GUILayout.Button("+") && pattern.leftMargin < manager.Height - 1)
            pattern.SetCoffset(pattern.leftMargin + 1);
        GUILayout.EndHorizontal();

        // topMargin
        GUILayout.BeginHorizontal();
        pattern.SetRoffset(EditorGUILayout.IntField("Top Margin", pattern.topMargin));
        if (GUILayout.Button("-") && pattern.topMargin > 0)
            pattern.SetRoffset(pattern.topMargin - 1);
        if (GUILayout.Button("+") && pattern.topMargin < manager.Width - 1)
            pattern.SetRoffset(pattern.topMargin + 1);
        GUILayout.EndHorizontal();

        // rightMargin
        GUILayout.BeginHorizontal();
        pattern.SetCendoffset(EditorGUILayout.IntField("Right Margin", pattern.rightMargin));
        if (GUILayout.Button("-") && pattern.rightMargin > 0)
            pattern.SetCendoffset(pattern.rightMargin - 1);
        if (GUILayout.Button("+") && manager.Height - pattern.rightMargin > pattern.leftMargin + 1)
            pattern.SetCendoffset(pattern.rightMargin + 1);
        GUILayout.EndHorizontal();

        // bottomMargin
        GUILayout.BeginHorizontal();
        pattern.SetRendoffset(EditorGUILayout.IntField("Bottom Margin", pattern.bottomMargin));
        if (GUILayout.Button("-") && pattern.bottomMargin > 0)
            pattern.SetRendoffset(pattern.bottomMargin - 1);
        if (GUILayout.Button("+") && manager.Width - pattern.bottomMargin > pattern.topMargin + 1)
            pattern.SetRendoffset(pattern.bottomMargin + 1);
        GUILayout.EndHorizontal();

        // verticalStep
        GUILayout.BeginHorizontal();
        pattern.SetSpacing(EditorGUILayout.IntField("Vert. Step", pattern.verticalStep));
        if (GUILayout.Button("-") && pattern.verticalStep > -1)
            pattern.SetSpacing(pattern.verticalStep - 1);
        if (GUILayout.Button("+"))
            pattern.SetSpacing(pattern.verticalStep + 1);
        GUILayout.EndHorizontal();

        // horizontalStep
        GUILayout.BeginHorizontal();
        pattern.SetRepeat(EditorGUILayout.IntField("Horiz. Step", pattern.horizontalStep));
        if (GUILayout.Button("-") && pattern.horizontalStep > -1)
            pattern.SetRepeat(pattern.horizontalStep - 1);
        if (GUILayout.Button("+"))
            pattern.SetRepeat(pattern.horizontalStep + 1);
        GUILayout.EndHorizontal();

    }

    private void DoCanvas()
    {
        Color oldColor = GUI.color;
        GUILayout.BeginVertical();

        int currentTileIndex;
        Color tileColor;

        for (int h = 0; h < manager.Height; ++h)
        {
            GUILayout.BeginHorizontal();

            for (int w = 0; w < manager.Width; ++w)
            {
                currentTileIndex = manager.TileIndex(w, h);
                tileColor = manager.tiles[currentTileIndex];

                if (manager.tilesInversion[currentTileIndex]) {
                    tileColor = tileColor.Invert();
                }
                GUI.color = tileColor;

                Texture2D tex = EditorGUIUtility.whiteTexture;

                // reserve a rect in the GUI layout system
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                // debug
                // Debug.Log("Rect: " + rect.x + ", " + rect.y);
                
                // then use it
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                // draw borders around the tile if it is locked
                if (manager.TileIsLocked(w, h))
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
        GUILayout.BeginVertical(GUILayout.MaxWidth(LevelInfo.MaxGridPanelWidth));

        GUILayout.Label("Toolbar", EditorStyles.largeLabel);

        // horizontal line separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ----- dimensions of the grid
        GUILayout.Label("Grid dimensions", EditorStyles.label);

        // width
        GUILayout.BeginHorizontal();
        // TODO: check wether this first binding is useful or not
        manager.Width = EditorGUILayout.IntField("Width", manager.Width);

        if (GUILayout.Button("-"))
            manager.Width -= 1;
        if (GUILayout.Button("+"))
            manager.Width += 1;
        GUILayout.EndHorizontal();

        // height
        GUILayout.BeginHorizontal();
        manager.Height = EditorGUILayout.IntField("Height", manager.Height);
        if (GUILayout.Button("-"))
        {
            manager.Height -= 1;
            foreach (ScriptableTilePattern pattern in manager.patterns)
            {
                pattern.SetHeight(manager.Height);
            }
        }
        if (GUILayout.Button("+"))
        {
            manager.Height += 1;
            foreach (ScriptableTilePattern pattern in manager.patterns)
            {
                pattern.SetHeight(manager.Height);
            }
        }
        GUILayout.EndHorizontal();

        // horizontal line separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ----- colors
        GUILayout.Label("Corner colors", EditorStyles.label);

        // colors of the corners
        GUILayout.BeginHorizontal();
        manager.TopLeft = EditorGUILayout.ColorField(manager.TopLeft, GUILayout.MaxWidth(LevelInfo.MaxSidePanelWidth));
        manager.TopRight = EditorGUILayout.ColorField(manager.TopRight, GUILayout.MaxWidth(LevelInfo.MaxSidePanelWidth));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        manager.BottomLeft = EditorGUILayout.ColorField(manager.BottomLeft, GUILayout.MaxWidth(LevelInfo.MaxSidePanelWidth));
        manager.BottomRight = EditorGUILayout.ColorField(manager.BottomRight, GUILayout.MaxWidth(LevelInfo.MaxSidePanelWidth));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // randomize colors button
        if (GUILayout.Button("Randomize Colors")) {
            manager.RandomizeColors();
        }

        // horizontal line separator
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // ----- fixed tiles
        GUILayout.Label("Fixed Tiles Patterns", EditorStyles.label);

        EditorGUILayout.Space();

        // render each pattern
        for (int i = 0; i < manager.patterns.Count; ++i)
        {
            DoPatternAt(i);
        }

        // add new pattern
        if (GUILayout.Button("(+) New Pattern"))
        {
            manager.patterns.Add(ScriptableObject.CreateInstance<ScriptableTilePattern>());
            manager.patterns[manager.patterns.Count - 1].SetHeight(manager.Height);
            manager.patterns[manager.patterns.Count - 1].SetWidth(manager.Width);
        }

        // save button
        if (GUILayout.Button("Save as..."))
        {
            //string path = EditorUtility.SaveFilePanel("Save level as", "Assets/Levels", "level.json", "json");
            string path = EditorUtility.SaveFilePanel("Save level as", "Assets/Levels", "level.asset", "asset");

            if (path.Length !=0)
            {
                SaveLevel(path);
            }
        }

        // TODO level id selector

        GUILayout.EndVertical();
    }

    void HandleMouseUp(Event evt)
    {
        // always 300
        //Debug.Log(GUILayoutUtility.GetLastRect().x + GUILayoutUtility.GetLastRect().width);
        // Debug.Log("Handling mouse up on editor window at position " + evt.mousePosition.x + ", " + evt.mousePosition.y);
        // Debug.Log("Window width: " + position.width + ", height: " + position.height);

        float xf = ((int)evt.mousePosition.x - LevelInfo.MaxGridPanelWidth) * manager.Width / (position.width - LevelInfo.MaxGridPanelWidth);
        int x = (xf > 0? (int)xf : -1);
        int y = (int) ( (int)evt.mousePosition.y * manager.Height / position.height);

        if (x >= 0 && y >= 0) {
            manager.InvertTilesAround(x, y);
            this.Repaint();
        }
    }
}
