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
        float red = GetBarycenter(corners[0].r, corners[1].r, corners[2].r, corners[3].r, h, w);
        float green = GetBarycenter(corners[0].g, corners[1].g, corners[2].g, corners[3].g, h, w);
        float blue = GetBarycenter(corners[0].b, corners[1].b, corners[2].b, corners[3].b, h, w);

        return new Color(red, green, blue, 1f);
    }

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

    // called whenever the editor window is created
    public void OnEnable()
    {
        width = height = 8;
        corners = new Color[4];
        tiles = new Color[width * height];

        for (int i = 0; i < 4; ++i)
        {
            corners[i] = new Color(Random.value, Random.value, Random.value, 1f);
        }
    }

    private void DoCanvas()
    {
        Color oldColor = GUI.color;
        GUILayout.BeginVertical();

        for (int w = 0; w < width; ++w)
        {
            GUILayout.BeginHorizontal();

            for (int h = 0; h < height; ++h)
            {
                tiles[h*width + w] = ComputeColor(w, h);
                GUI.color = tiles[h * width + w];
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

    private void DoControls()
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Toolbar", EditorStyles.largeLabel);

        corners[0] = EditorGUILayout.ColorField("Bottom-left color", corners[0]);
        corners[1] = EditorGUILayout.ColorField("Top-left color", corners[1]);
        corners[2] = EditorGUILayout.ColorField("Top-right color", corners[2]);
        corners[3] = EditorGUILayout.ColorField("Bottom-right color", corners[3]);

        if (GUILayout.Button("Randomize")) {
            for (int i = 0; i < 4; ++i)
            {
                corners[i] = new Color(Random.value, Random.value, Random.value, 1f);
            }

            //ApplyColors();
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
