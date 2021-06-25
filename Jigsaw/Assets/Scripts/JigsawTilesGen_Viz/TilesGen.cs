using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle;

public class TilesGen : MonoBehaviour
{
    public string ImageFilename;

    private Texture2D mTextureOriginal;

    void Start()
    {
        CreateBaseTexture();
        //TestTileCurves();
        TestTileFloodFill();
    }

    void CreateBaseTexture()
    {
        // Load the main image.
        mTextureOriginal = SpriteUtils.LoadTexture(ImageFilename);
        if (!mTextureOriginal.isReadable)
        {
            Debug.Log("Texture is not readable");
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

    void TestTileCurves()
    {
        Tile tile = new Tile(mTextureOriginal);
        tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.NONE, Color.white);

        tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.NONE, Color.white);

        tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.NONE, Color.white);

        tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.POS, Color.red);
        tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.NEG, Color.green);
        tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.NONE, Color.white);
    }

    void TestTileFloodFill()
    {
        Tile tile = new Tile(mTextureOriginal);

        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NEG);
        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
        tile.SetPosNegType(Tile.Direction.DOWN, Tile.PosNegType.NEG);
        tile.SetPosNegType(Tile.Direction.LEFT, Tile.PosNegType.NEG);

        // Uncomment the following 4 lines of code if you want to see the 
        // curves drawn on the tile too.
        tile.DrawCurve(Tile.Direction.UP, Tile.PosNegType.NEG, Color.white);
        tile.DrawCurve(Tile.Direction.RIGHT, Tile.PosNegType.NONE, Color.white);
        tile.DrawCurve(Tile.Direction.DOWN, Tile.PosNegType.NEG, Color.white);
        tile.DrawCurve(Tile.Direction.LEFT, Tile.PosNegType.NEG, Color.white);

        tile.Apply();
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
            tile.FinalCut, 
            0, 
            0,
            tile.FinalCut.width,
            tile.FinalCut.height);
        spriteRenderer.sprite = sprite;
    }
}
