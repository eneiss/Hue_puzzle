using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableTilePattern : ScriptableObject
{

    // pattern parameters
    public int roffset, coffset, rendoffset, cendoffset, spacing, repeat;

    // coordinates of each fixed tile: (row, column)
    List<Tuple<int, int>> tiles = null;

    // compute the coordinates of each fixed tile in the pattern
    void ComputeTileCoords()
    {
        tiles = new List<Tuple<int, int>>();

        GameObject levelDataObject = GameObject.FindWithTag("LevelData");
        LevelData levelData = (LevelData)levelDataObject.GetComponent(typeof(LevelData));


        /* TODO
         * compute all row indexes where tiles are locked
         * compute all column indexes (same)
         * add to the list all combinations of rows & columns above
         */

        List<int> rIndexes = new List<int>();
        List<int> cIndexes = new List<int>();

        // rows
        if (spacing > 0)
        {
            for (int i = roffset; i < levelData.nbRows - rendoffset; i += spacing)
            {
                rIndexes.Add(i);
            }
        }
        else
        {
            rIndexes.Add(roffset);
        }


        // columns
        if (repeat > 0)
        {
            for (int i = coffset; i < levelData.nbColumns - cendoffset; i += repeat)
            {
                cIndexes.Add(i);
            }
        }
        else
        {
            cIndexes.Add(coffset);
        }

        foreach (int r in rIndexes)
        {
            foreach (int c in cIndexes)
            {
                tiles.Add(new Tuple<int, int>(r, c));
            }
        }
    }

    public IEnumerator GetEnumerator()
    {
        if (tiles == null)
        {
            ComputeTileCoords();
        }

        foreach (Tuple<int, int> tuple in tiles)
        {
            yield return tuple;
        }
    }

    public void Copy(FixedTilePattern source)
    {
        this.roffset = source.roffset;
        this.coffset = source.coffset;
        this.rendoffset = source.rendoffset;
        this.cendoffset = source.cendoffset;
        this.spacing = source.spacing;
        this.repeat = source.repeat;

        // dont copy tiles since they were not computed yet
    }


    // ----- setters to use when needing to compute tile coordinates at every modification
    public void SetRoffset(int roffset)
    {
        this.roffset = roffset;
        ComputeTileCoords();
    }

    public void SetCoffset(int coffset)
    {
        this.coffset = coffset;
        ComputeTileCoords();
    }

    public void SetRendoffset(int rendoffset)
    {
        this.rendoffset = rendoffset;
        ComputeTileCoords();
    }

    public void SetCendoffset(int cendoffset)
    {
        this.cendoffset = cendoffset;
        ComputeTileCoords();
    }

    public void SetSpacing(int spacing)
    {
        this.spacing = spacing;
        ComputeTileCoords();
    }

    public void SetRepeat(int repeat)
    {
        this.repeat = repeat;
        ComputeTileCoords();
    }

}
