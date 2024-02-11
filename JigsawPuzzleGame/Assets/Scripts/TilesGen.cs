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
    DrawCurves();
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

  void DrawCurves()
  {

    //mTile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.POS, UnityEngine.Color.blue);
    //mTile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.POS, UnityEngine.Color.blue);
    //mTile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.POS, UnityEngine.Color.blue);
    //mTile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.POS, UnityEngine.Color.blue);
    mTile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.NEG, UnityEngine.Color.red);
    mTile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.NEG, UnityEngine.Color.red);
    mTile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.NEG, UnityEngine.Color.red);
    mTile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.NEG, UnityEngine.Color.red);
  }

  // Update is called once per frame
  void Update()
  {
  }
}
