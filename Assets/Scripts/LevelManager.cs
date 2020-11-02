using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject grid;
    public GameObject tilePrefab;
    GameObject[] tiles;
    List<(int dx, int dy)> neighbours;
    LevelData levelData;

    // Start is called before the first frame update
    void Start()
    {

        // relative pos of neighbour tiles 
        neighbours = new List<(int dx, int dy)>
        {
            (-1, -1), (-1, 1), (1, -1), (1, 1),
            (0, 1), (0, -1), (1, 0), (-1, 0), (0, 0)
        };

        levelData = (LevelData) GameObject.FindWithTag("LevelData").GetComponent(typeof(LevelData));
        //Debug.Log(levelData.nbRows);

        GenerateInitialGrid();

        tiles = GameObject.FindGameObjectsWithTag("Tile");

        ApplyMoves();

    }

    void GenerateInitialGrid()
    {
        for (int r = 0; r < levelData.nbRows; ++r)
        {
            for (int c = 0; c < levelData.nbColumns; ++c)
            {
                Color tileColor = GetTileColor(r, c);

                // todo instantiate a tile with the right color & pos that is a child of the grid
                GameObject tile = GameObject.Instantiate(tilePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, grid.transform);
                tile.transform.localPosition = new Vector3(r, c, 0f);
                tile.GetComponent<SpriteRenderer>().color = tileColor;

            }
        }
    }

    // r, c 0-based indexing
    Color GetTileColor(int r, int c)
    {
        float red = GetBarycenter(levelData.topLeftColor.r, levelData.topRightColor.r, levelData.bottomRightColor.r, levelData.bottomLeftColor.r, r, c);
        float green = GetBarycenter(levelData.topLeftColor.g, levelData.topRightColor.g, levelData.bottomRightColor.g, levelData.bottomLeftColor.g, r, c);
        float blue = GetBarycenter(levelData.topLeftColor.b, levelData.topRightColor.b, levelData.bottomRightColor.b, levelData.bottomLeftColor.b, r, c);

        return new Color(red, green, blue, 1f);
    }

    // call for each rbg composant and get the final value for the composant at pos r, c
    float GetBarycenter(float c1, float c2, float c3, float c4, int r, int c)
    {
        float x1 = c;
        float x2 = levelData.nbColumns - c - 1f;
        float y1 = r;
        float y2 = levelData.nbRows - r - 1f;

        // todo optimize by factorizing
        float c12 = (x1 * c1 + x2 * c2) / (x1 + x2);
        float c43 = (x1 * c4 + x2 * c3) / (x1 + x2);
        float c14 = (y2 * c1 + y1 * c4) / (y1 + y2);
        float c23 = (y2 * c2 + y1 * c3) / (y1 + y2);
        float cVertic = (y2 * c12 + y1 * c43) / (y1 + y2);
        float cHoriz = (x2 * c23 + x1 * c14) / (x1 + x2);

        return (cHoriz + cVertic) / 2;
    }

    void ApplyMoves()
    {
        // TODO apply the moves of the level data to the grid
        foreach (string move in levelData.moves)
        {
            Debug.Log(move);
            string[] splitString = move.Split(' ');
            Debug.Log("Move : " + Int16.Parse(splitString[0]) + ", " + Int16.Parse(splitString[1]));
            InvertTiles(Int16.Parse(splitString[0]), Int16.Parse(splitString[1]));
        }
    }

    public void InvertTiles(int x, int y)
    {
        bool allCorrect = true;

        foreach (GameObject tile in tiles)
        {
            foreach ((int dx, int dy) in neighbours)
            {
                //Debug.Log("dx : " + dx + ", dy : " + dy);

                if ((x + dx == tile.transform.localPosition.x) && (y + dy == tile.transform.localPosition.y))
                {
                    TileScript ts = (TileScript)tile.GetComponent(typeof(TileScript));
                    ts.InvertColor();
                    if (ts.isInverted)
                    {
                        allCorrect = false;
                    }
                }
            }
        }

        if (allCorrect)
        {
            if (LevelIsCleared())
            {
                EndLevel();
            }
        }
    }

    bool LevelIsCleared()
    {
        bool allCorrect = true;
        foreach (GameObject tile in tiles)
        {
            TileScript ts = (TileScript)tile.GetComponent(typeof(TileScript));
            if (ts.isInverted)
            {
                allCorrect = false;
                break;
            }
        }

        return allCorrect;
    }

    void EndLevel()
    {
        Debug.Log("GG!");
    }

}
