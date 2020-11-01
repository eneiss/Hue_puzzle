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
        foreach ((int dxVar, int dyVar) in neighbours)
        {
            Debug.Log("dx : " + dxVar + ", dy : " + dyVar);
        }
    }
}
