using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu, System.Serializable]
public class ScriptableTilePattern : ScriptableObject {

    // pattern parameters
    [SerializeField]
    public int topMargin, leftMargin, bottomMargin, rightMargin, verticalStep, horizontalStep;
    public int width, height;

    // coordinates of each fixed tile: (row, column)
    public List<Vector2Int> tiles;

    // compute the coordinates of each fixed tile in the pattern
    [ContextMenu("Compute Tile Coords")]
    void ComputeTileCoords() {
        tiles = new List<Vector2Int>();

        /*
         * compute all row indexes where tiles are locked
         * compute all column indexes (same)
         * add to the list all combinations of rows & columns above
         */

        List<int> rIndexes = new List<int>();
        List<int> cIndexes = new List<int>();

        // TODO: see if this could be sipmlified
        // rows
        if (verticalStep > 0) {
            for (int i = topMargin; i < height - bottomMargin; i += verticalStep) {
                rIndexes.Add(i);
            }
        }
        else {
            rIndexes.Add(topMargin);
        }


        // columns
        if (horizontalStep > 0) {
            for (int i = leftMargin; i < width - rightMargin; i += horizontalStep) {
                cIndexes.Add(i);
            }
        }
        else {
            cIndexes.Add(leftMargin);
        }

        foreach (int r in rIndexes) {
            foreach (int c in cIndexes) {
                tiles.Add(new Vector2Int(r, c));
            }
        }
    }

    public IEnumerator GetEnumerator() {
        if (tiles == null) {
            ComputeTileCoords();
        }

        foreach (Vector2Int tuple in tiles) {
            yield return tuple;
        }
    }

    public void Copy(FixedTilePattern source) {
        this.topMargin = source.roffset;
        this.leftMargin = source.coffset;
        this.bottomMargin = source.rendoffset;
        this.rightMargin = source.cendoffset;
        this.verticalStep = source.spacing;
        this.horizontalStep = source.repeat;

        // don't copy tiles since they were not computed yet
    }


    // ----- setters to use when needing to compute tile coordinates at every modification
    public void SetRoffset(int roffset) {
        this.topMargin = roffset;
        ComputeTileCoords();
    }

    public void SetCoffset(int coffset) {
        this.leftMargin = coffset;
        ComputeTileCoords();
    }

    public void SetRendoffset(int rendoffset) {
        this.bottomMargin = rendoffset;
        ComputeTileCoords();
    }

    public void SetCendoffset(int cendoffset) {
        this.rightMargin = cendoffset;
        ComputeTileCoords();
    }

    public void SetSpacing(int spacing) {
        this.verticalStep = spacing;
        ComputeTileCoords();
    }

    public void SetRepeat(int repeat) {
        this.horizontalStep = repeat;
        ComputeTileCoords();
    }

    public void SetWidth(int width) {
        this.width = width;
        ComputeTileCoords();
    }

    public void SetHeight(int height) {
        this.height = height;
        ComputeTileCoords();
    }
}