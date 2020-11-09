using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedTilePattern : MonoBehaviour, IEnumerable
{

    public int roffset, coffset, rendoffset, cendoffset, spacing, repeat;
    // coordinates of each fixed tile: (row, column)
    List<Tuple<int, int>> tiles = null;

    // computes the coordinates of each fixed tile in the pattern
    void computeTileCoords()
    {
        // todo
        tiles.Add(new Tuple<int, int>(roffset, coffset));
    }

    public IEnumerator GetEnumerator()
    {
        if (tiles == null)
        {
            computeTileCoords();
        }

        foreach (Tuple<int, int> tuple in tiles)
        {
            yield return (tuple.Item1, tuple.Item2);
        }
    }

}
