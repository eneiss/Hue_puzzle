using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorWindow : EditorWindow
{
    private Color[] corners;
    private Color[] tiles;
    private int width;
    private int height;

    private Color selectedColor;
    const int MAX_HEIGHT = 20;
    const int MAX_WIDTH = 20;
    const int MAX_COLOR_WIDTH = 100;
    const int MAX_PANEL_WIDTH = 300;

    [MenuItem("Custom/Level Editor")]
    public static void ShowLevelEditorWindow()
    {
        // find window if existing, else create it
        EditorWindow window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("Level Editor");
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
        return (MAX_WIDTH * h) + w;
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

    // called whenever the editor window is created
    public void OnEnable()
    {
        width = height = 8;
        corners = new Color[4];
        tiles = new Color[MAX_WIDTH * MAX_HEIGHT];

        for (int i = 0; i < 4; ++i)
        {
            corners[i] = new Color(Random.value, Random.value, Random.value, 1f);
        }
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
                // reserve a rect in the GUI layout system
                var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                // then use it
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUI.color = oldColor;
    }

    // side tools panel
    private void DoControls()
    {
        GUILayout.BeginVertical(GUILayout.MaxWidth(MAX_PANEL_WIDTH));
        GUILayout.Label("Toolbar", EditorStyles.largeLabel);

        // ----- dimensions of the grid
        GUILayout.Label("Dimensions", EditorStyles.label);

        // width
        GUILayout.BeginHorizontal();
        width = EditorGUILayout.IntField("Width", width);
        if (GUILayout.Button("-") && width > 1)
            width -= 1;
        if (GUILayout.Button("+") && width < MAX_WIDTH)
            width += 1;
        GUILayout.EndHorizontal();

        // height
        GUILayout.BeginHorizontal();
        height = EditorGUILayout.IntField("Height", height);
        if (GUILayout.Button("-") && height > 1)
            height -= 1;
        if (GUILayout.Button("+") && width < MAX_HEIGHT)
            height += 1;
        GUILayout.EndHorizontal();

        // check for bounds
        if (width < 0)
            width = 0;
        if (width > MAX_WIDTH)
            width = MAX_WIDTH;
        if (height < 0)
            height = 0;
        if (height > MAX_HEIGHT)
            height = MAX_HEIGHT;

        // ----- colors
        GUILayout.Label("Colors", EditorStyles.label);

        // colors of the corners
        GUILayout.BeginHorizontal();
        corners[1] = EditorGUILayout.ColorField(corners[1], GUILayout.MaxWidth(MAX_COLOR_WIDTH));
        corners[2] = EditorGUILayout.ColorField(corners[2], GUILayout.MaxWidth(MAX_COLOR_WIDTH));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        corners[0] = EditorGUILayout.ColorField(corners[0], GUILayout.MaxWidth(MAX_COLOR_WIDTH));
        corners[3] = EditorGUILayout.ColorField(corners[3], GUILayout.MaxWidth(MAX_COLOR_WIDTH));
        GUILayout.EndHorizontal();
        //corners[1] = EditorGUILayout.ColorField("Top-left color", corners[1]);
        //corners[2] = EditorGUILayout.ColorField("Top-right color", corners[2]);
        //corners[3] = EditorGUILayout.ColorField("Bottom-right color", corners[3]);
        //corners[0] = EditorGUILayout.ColorField("Bottom-left color", corners[0]);

        // randomize button
        if (GUILayout.Button("Randomize")) {
            for (int i = 0; i < 4; ++i)
            {
                corners[i] = new Color(Random.value, Random.value, Random.value, 1f);
            }
        }

        GUILayout.EndVertical();
    }

    // called once per frame -> no need to explicitely call ApplyColors() ?
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        DoControls();

        DoCanvas();
        GUILayout.EndHorizontal();
    }
}
