using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGen : MonoBehaviour
{
  public string imageFilename;
  private Texture2D mTextureOriginal;

  Tile mTile = new Tile();

  // Start is called before the first frame update
  void Start()
  {
    CreateBaseTexture();

    //mTile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.POS, UnityEngine.Color.blue);
    //mTile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.POS, UnityEngine.Color.blue);
    //mTile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.POS, UnityEngine.Color.blue);
    //mTile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.POS, UnityEngine.Color.blue);
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
    spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
      mTextureOriginal,
      0,
      0,
      mTextureOriginal.width,
      mTextureOriginal.height);
  }

  private (Tile.PosNegType, UnityEngine.Color) GetRandomType()
  {
    Tile.PosNegType type = Tile.PosNegType.POS;
    float rand = UnityEngine.Random.Range(0f, 1f);
    UnityEngine.Color color = UnityEngine.Color.blue;

    if (rand < 0.5f)
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
      mTile.HideAllCurves();
      var type_color = GetRandomType();
      mTile.DrawCurve(Tile.Direction.UP, type_color.Item1, type_color.Item2);
      type_color = GetRandomType();
      mTile.DrawCurve(Tile.Direction.RIGHT, type_color.Item1, type_color.Item2);
      type_color = GetRandomType();
      mTile.DrawCurve(Tile.Direction.DOWN, type_color.Item1, type_color.Item2);
      type_color = GetRandomType();
      mTile.DrawCurve(Tile.Direction.LEFT, type_color.Item1, type_color.Item2);
    }
  }
}
