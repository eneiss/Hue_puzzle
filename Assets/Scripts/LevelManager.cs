using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject grid;
    public GameObject tilePrefab;
    GameObject[] tiles;
    List<(int dx, int dy)> neighbours;
    LevelData levelData;
    double screenOccupation = 0.95;

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

        SetFixedTiles();

        ApplyMoves();

        ScaleAndMoveGrid();

    }

    // todo scale for portrait mode too ?
    void ScaleAndMoveGrid()
    {
        double height = 2 * Camera.main.orthographicSize;
        double width = height * Camera.main.aspect;

        GameObject grid = GameObject.FindGameObjectWithTag("Grid");

        float scale = (float)(width * screenOccupation / levelData.nbColumns);
        float xGrid = (-((float)(width * screenOccupation)) + scale) / 2f;
        float yGrid = (-((float)(height - scale*levelData.nbRows)) + scale) / 2f;

        grid.transform.localScale = new Vector3(scale, scale, scale);
        grid.transform.localPosition = new Vector3(xGrid, yGrid);
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
                tile.transform.localPosition = new Vector3(c, r, 0f);
                tile.GetComponent<SpriteRenderer>().color = tileColor;

            }
        }
    }

    void SetFixedTiles()
    {
        foreach (FixedTilePattern pattern in levelData.fixedTiles)
        {
            foreach (Tuple<int, int> coords in pattern)
            {
                GameObject tile = GetTile(coords.Item1, coords.Item2);
                TileScript tileScript = (TileScript) tile.GetComponent(typeof(TileScript));
                tileScript.SetFixed(true);
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

    GameObject GetTile(int r, int c)
    {
        foreach (GameObject tile in tiles)
        {
            // !!!! conflicts with the rest of the program (differents coordinates)
            // if ((tile.transform.localPosition.x == r) && (levelData.nbColumns - tile.transform.localPosition.y - 1 == c))
            if ((tile.transform.localPosition.x == r) && (tile.transform.localPosition.y == c))
            {
                return tile;
            }
        }

        return null;
    }
         
    // call for each rbg component and get the final value for the component at pos r, c
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
            string[] splitString = move.Split(' ');
            int r = Int16.Parse(splitString[0]);
            int c = Int16.Parse(splitString[1]);
            Debug.Log("Move : " + r + ", " + c);

            TileScript ts = (TileScript) GetTile(r, c).GetComponent<TileScript>();

            if (ts.GetFixed())
            {
                Debug.Log("WARNING: trying to apply a move on a fixed tile !");
            } else
            {
                InvertTiles(r, c);
                Debug.Log("Applying move at " + r + ", " + c);
            }
        }
    }

    public void InvertTiles(int x, int y)
    {
        Debug.Log("Click at " + x + ", " + y);

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
            TileScript ts = (TileScript) tile.GetComponent(typeof(TileScript));
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
