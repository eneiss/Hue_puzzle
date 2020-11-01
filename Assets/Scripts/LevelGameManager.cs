using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using UnityEngine;

public class LevelGameManager : MonoBehaviour
{

    GameObject[] tiles;
    List<(int dx, int dy)> neighbours;

    // Start is called before the first frame update
    void Start()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        // relative pos of neighbour tiles 
        neighbours = new List<(int dx, int dy)>
        {
            (-1, -1), (-1, 1), (1, -1), (1, 1),
            (0, 1), (0, -1), (1, 0), (-1, 0)
        };
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
                    TileScript ts = (TileScript) tile.GetComponent(typeof(TileScript));
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
