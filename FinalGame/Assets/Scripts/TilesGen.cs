using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGen : MonoBehaviour
{
  public string imageFilename;
  private Texture2D mTextureOriginal;

  private Tile mTile = null;
  private Sprite mSprite = null;

  // Start is called before the first frame update
  void Start()
  {
    CreateBaseTexture();
  }

  void CreateBaseTexture()
  {
    mTextureOriginal = SpriteUtils.LoadTexture(imageFilename);
    if(!mTextureOriginal.isReadable)
    {
      Debug.Log("Texture is nor readable");
      return;
    }

    SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
    mSprite = SpriteUtils.CreateSpriteFromTexture2D(
      mTextureOriginal,
      0,
      0,
      mTextureOriginal.width,
      mTextureOriginal.height);
    spriteRenderer.sprite = mSprite;
  }

  private (Tile.PosNegType, UnityEngine.Color) GetRendomType()
  {
    Tile.PosNegType type = Tile.PosNegType.POS;
    UnityEngine.Color color = UnityEngine.Color.blue;
    float rand = UnityEngine.Random.Range(0f, 1f);

    if(rand < 0.5f)
    {
      type = Tile.PosNegType.POS;
      color = UnityEngine.Color.blue;
    }
    else
    {
      type = Tile.PosNegType.NEG;
      color = UnityEngine.Color.red;
    }
    return (type, color);
  }

  // Update is called once per frame
  void Update()
  {
    if(Input.GetKeyDown(KeyCode.Space))
    {
      TestRandomCurves();
    }
    else if(Input.GetKeyDown(KeyCode.F))
    {
      TestTileFloodFill();
    }
  }

  void TestRandomCurves()
  {
    if(mTile != null)
    {
      mTile.DestroyAllCurves();
      mTile = null;
    }

    Tile tile = new Tile(mTextureOriginal);
    mTile = tile;

    var type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.UP, type_color.Item1, type_color.Item2);
    type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.RIGHT, type_color.Item1, type_color.Item2);
    type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.DOWN, type_color.Item1, type_color.Item2);
    type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.LEFT, type_color.Item1, type_color.Item2);

    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.sprite = mSprite;

  }

  void TestTileFloodFill()
  {
    if(mTile != null)
    {
      mTile.DestroyAllCurves();
      mTile = null;
    }

    mTile = new Tile(mTextureOriginal);


    var type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.UP, type_color.Item1, type_color.Item2);
    mTile.SetCurveType(Tile.Direction.UP, type_color.Item1);

    type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.RIGHT, type_color.Item1, type_color.Item2);
    mTile.SetCurveType(Tile.Direction.RIGHT, type_color.Item1);

    type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.DOWN, type_color.Item1, type_color.Item2);
    mTile.SetCurveType(Tile.Direction.DOWN, type_color.Item1);

    type_color = GetRendomType();
    mTile.DrawCurve(Tile.Direction.LEFT, type_color.Item1, type_color.Item2);
    mTile.SetCurveType(Tile.Direction.LEFT, type_color.Item1);

    mTile.Apply();

    // We will now set the texture finalCut to the sprite.
    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
      mTile.finalCut,
      0,
      0,
      mTile.finalCut.width,
      mTile.finalCut.height);

    spriteRenderer.sprite = sprite;
  }
}
