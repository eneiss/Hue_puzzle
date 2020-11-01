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
        neighbours = new List<(int dx, int dy)>
        {
            (-1, -1), (-1, 1), (1, -1), (1, 1),     // diag
            (0, 1), (0, -1), (1, 0), (-1, 0)
        };
    }

    public void InvertTiles(int x, int y)
    {
        foreach (GameObject tile in tiles)
        {
            foreach ((int dx, int dy) in neighbours)
            {
                Debug.Log("dx : " + dx + ", dy : " + dy);

                if ((x + dx == tile.transform.localPosition.x) && (y + dy == tile.transform.localPosition.y))
                {
                    TileScript ts = (TileScript) tile.GetComponent(typeof(TileScript));
                    ts.InvertColor();
                }
            }
        }
    }
}
