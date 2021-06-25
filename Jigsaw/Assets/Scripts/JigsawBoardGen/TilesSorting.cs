using System.Collections.Generic;
using UnityEngine;

// the facilitate sorting of tiles.
public class TilesSorting
{
    private List<SpriteRenderer> mSortIndices = new List<SpriteRenderer>();
    public TilesSorting()
    {

    }

    public void Clear()
    {
        mSortIndices.Clear();
    }

    public void Add(SpriteRenderer ren)
    {
        mSortIndices.Add(ren);
        SetRenderOrder(ren, mSortIndices.Count);
    }

    public void Remove(SpriteRenderer ren)
    {
        mSortIndices.Remove(ren);
        for (int i = 0; i < mSortIndices.Count; ++i)
        {
            SetRenderOrder(mSortIndices[i], i + 1);
        }
    }

    public void BringToTop(SpriteRenderer ren)
    {
        // Find the index of ren.
        Remove(ren);
        Add(ren);
    }

    private void SetRenderOrder(SpriteRenderer ren, int order)
    {
        // First we set the render order of sorting.
        ren.sortingOrder = order;

        // Then we set the z value so that selection/raycast 
        // selects the top sprite.
        Vector3 p = ren.transform.position;
        p.z = -order / 10.0f;
        ren.transform.position = p;
    }
}
