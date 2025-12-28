using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  public void Add(SpriteRenderer renderer)
  {
    mSortIndices.Add(renderer);
    SetRenderOrder(renderer, mSortIndices.Count);
  }

  public void Remove(SpriteRenderer renderer)
  {
    mSortIndices.Remove(renderer);
    for(int i = 0; i < mSortIndices.Count; i++)
    {
      SetRenderOrder(mSortIndices[i], i + 1);
    }
  }

  public void BringToTop(SpriteRenderer renderer)
  {
    Remove(renderer);
    Add(renderer);
  }

  private void SetRenderOrder(SpriteRenderer renderer, int index)
  {
    renderer.sortingOrder = index;
    Vector3 p = renderer.transform.position;
    p.z = -index / 10.0f;
    renderer.transform.position = p;
  }
}
