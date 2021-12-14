﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu, System.Serializable]
public class ScriptableTilePattern : ScriptableObject
{

    // pattern parameters
    [SerializeField]
    public int topMargin, leftMargin, bottomMargin, rightMargin, verticalStep, horizontalStep;
    public int width, height;

    // coordinates of each fixed tile: (row, column)
    // TODO: use Vector2 instead for serialization
    public List<Tuple<int, int>> tiles = null;

    public List<Vector2> vector2s = new List<Vector2> { new Vector2(x: 3f, y: 42f) };

    [SerializeField]
    private Color color = new Color(0.5f, 0f, 0.5f);


    // compute the coordinates of each fixed tile in the pattern
    void ComputeTileCoords()
    {
        tiles = new List<Tuple<int, int>>();

        /*
         * compute all row indexes where tiles are locked
         * compute all column indexes (same)
         * add to the list all combinations of rows & columns above
         */

        List<int> rIndexes = new List<int>();
        List<int> cIndexes = new List<int>();

        // TODO: see if this could be sipmlified
        // rows
        if (verticalStep > 0)
        {
            for (int i = topMargin; i < height - bottomMargin; i += verticalStep)
            {
                rIndexes.Add(i);
            }
        }
        else
        {
            rIndexes.Add(topMargin);
        }


        // columns
        if (horizontalStep > 0)
        {
            for (int i = leftMargin; i < width - rightMargin; i += horizontalStep)
            {
                cIndexes.Add(i);
            }
        }
        else
        {
            cIndexes.Add(leftMargin);
        }

        foreach (int r in rIndexes)
        {
            foreach (int c in cIndexes)
            {
                // TODO: replace with Vector2
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
        this.topMargin = source.roffset;
        this.leftMargin = source.coffset;
        this.bottomMargin = source.rendoffset;
        this.rightMargin = source.cendoffset;
        this.verticalStep = source.spacing;
        this.horizontalStep = source.repeat;

        // don't copy tiles since they were not computed yet
    }


    // ----- setters to use when needing to compute tile coordinates at every modification
    public void SetRoffset(int roffset)
    {
        this.topMargin = roffset;
        ComputeTileCoords();
    }

    public void SetCoffset(int coffset)
    {
        this.leftMargin = coffset;
        ComputeTileCoords();
    }

    public void SetRendoffset(int rendoffset)
    {
        this.bottomMargin = rendoffset;
        ComputeTileCoords();
    }

    public void SetCendoffset(int cendoffset)
    {
        this.rightMargin = cendoffset;
        ComputeTileCoords();
    }

    public void SetSpacing(int spacing)
    {
        this.verticalStep = spacing;
        ComputeTileCoords();
    }

    public void SetRepeat(int repeat)
    {
        this.horizontalStep = repeat;
        ComputeTileCoords();
    }

    public void SetWidth(int width)
    {
        this.width = width;
        ComputeTileCoords();
    }

    public void SetHeight(int height)
    {
        this.height = height;
        ComputeTileCoords();
    }
}