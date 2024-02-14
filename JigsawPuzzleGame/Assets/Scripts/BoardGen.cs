using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGen : MonoBehaviour
{
  public string imageFilename;

  // The opaque sprite. 
  Sprite mBaseSpriteOpaque;

  // The transparent (or Ghost sprite)
  Sprite mBaseSpriteTransparent;

  // The game object that holds the opaque sprite.
  // This should be SetActive to false by default.
  GameObject mGameObjectOpaque;

  // The game object that holds the transparent sprite.
  GameObject mGameObjectTransparent;

  public float ghostTransparency = 0.2f;

  // Jigsaw tiles creation.
  public int numTilesX { get; private set; }
  public int numTilesY { get; private set; }
  Tile[,] mTiles = null;
  GameObject[,] mTileGameObjects = null; 
  public Transform parentForTiles = null;

  private void Start()
  {
    mBaseSpriteOpaque = LoadBaseTexture();
    mGameObjectOpaque = new GameObject();
    mGameObjectOpaque.name = imageFilename + "_Opaque";
    mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
    mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

    mBaseSpriteTransparent = CreateTransparentView(mBaseSpriteOpaque.texture);
    mGameObjectTransparent = new GameObject();
    mGameObjectTransparent.name = imageFilename + "_Transparent";
    mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
    mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

    // Hide the mBaseSpriteOpaque game object.
    mGameObjectOpaque.gameObject.SetActive(false);

    SetCameraPosition();
    StartCoroutine(Coroutine_CreateJigsawTiles());
  }

  void SetCameraPosition()
  {
    Camera.main.transform.position = new Vector3(mBaseSpriteOpaque.texture.width / 2,
      mBaseSpriteOpaque.texture.height / 2, -10.0f);
    Camera.main.orthographicSize = mBaseSpriteOpaque.texture.width / 2;
  }

  Sprite LoadBaseTexture()
  {
    Texture2D tex = SpriteUtils.LoadTexture(imageFilename);
    if (!tex.isReadable)
    {
      Debug.Log("Error: Texture is not readable");
      return null;
    }

    if (tex.width % Tile.tileSize != 0 || tex.height % Tile.tileSize != 0)
    {
      Debug.Log("Error: Image must be of size that is multiple of <" + Tile.tileSize + ">");
      return null;
    }

    // Add padding to the image.
    Texture2D newTex = new Texture2D(
        tex.width + Tile.padding * 2,
        tex.height + Tile.padding * 2,
        TextureFormat.ARGB32,
        false);

    // Set the default colour as white
    for (int x = 0; x < newTex.width; ++x)
    {
      for (int y = 0; y < newTex.height; ++y)
      {
        newTex.SetPixel(x, y, Color.white);
      }
    }

    // Copy the colours.
    for (int x = 0; x < tex.width; ++x)
    {
      for (int y = 0; y < tex.height; ++y)
      {
        Color color = tex.GetPixel(x, y);
        color.a = 1.0f;
        newTex.SetPixel(x + Tile.padding, y + Tile.padding, color);
      }
    }
    newTex.Apply();

    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
        newTex,
        0,
        0,
        newTex.width,
        newTex.height);
    return sprite;
  }

  Sprite CreateTransparentView(Texture2D tex)
  {
    // Add padding to the image.
    Texture2D newTex = new Texture2D(
        tex.width,
        tex.height,
        TextureFormat.ARGB32,
        false);

    //for (int x = Tile.padding; x < Tile.padding + Tile.tileSize; ++x)
    for (int x = 0; x < newTex.width; ++x)
    {
      //for (int y = Tile.padding; y < Tile.padding + Tile.tileSize; ++y)
      for (int y = 0; y < newTex.height; ++y)
      {
        Color c = tex.GetPixel(x, y);
        if (x > Tile.padding && x < (newTex.width - Tile.padding) && y > Tile.padding && y < newTex.height - Tile.padding)
        {
          c.a = ghostTransparency;
        }
        newTex.SetPixel(x, y, c);
      }
    }

    newTex.Apply();

    Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(
        newTex,
        0,
        0,
        newTex.width,
        newTex.height);
    return sprite;
  }
  public static GameObject CreateGameObjectFromTile(Tile tile)
  {
    // Create a game object for the tile.
    GameObject obj = new GameObject();

    // Give a name that is recognizable for the GameObject.
    obj.name = "TileGameObj_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

    // Set the position of this GameObject.
    // We will use the xIndex and yIndex to find the actual 
    // position of the tile. We can get this position by multiplying
    // xIndex by tileSize and yIndex by tileSize.
    obj.transform.position = new Vector3(tile.xIndex * Tile.tileSize, tile.yIndex * Tile.tileSize, 0.0f);

    // Create a SpriteRenderer.
    SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();

    // Set the sprite created with the FinalCut 
    // texture of the tile to the SpriteRenderer
    spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
        tile.finalCut,
        0,
        0,
        Tile.padding * 2 + Tile.tileSize,
        Tile.padding * 2 + Tile.tileSize);

    // Add a box colliders so that we can handle 
    // picking/selection of the Tiles.
    BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

    return obj;
  }

  Tile CreateTile(int i, int j, Texture2D baseTexture)
  {
    Tile tile = new Tile(baseTexture);
    tile.xIndex = i;
    tile.yIndex = j;

    // Left side tiles
    if (i == 0)
    {
      tile.SetCurveType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
    }
    else
    {
      // We have to create a tile that has LEFT direction opposite operation 
      // of the tile on the left's RIGHT direction operation.
      Tile leftTile = mTiles[i - 1, j];
      Tile.PosNegType rightOp = leftTile.GetCurveType(Tile.Direction.RIGHT);
      tile.SetCurveType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
    }

    // Bottom side tiles
    if (j == 0)
    {
      tile.SetCurveType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
    }
    else
    {
      // We have to create a tile that has LEFT direction opposite operation 
      // of the tile on the left's RIGHT direction operation.
      Tile downTile = mTiles[i, j - 1];
      Tile.PosNegType rightOp = downTile.GetCurveType(Tile.Direction.UP);
      tile.SetCurveType(Tile.Direction.DOWN, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
    }

    // Right side tiles
    if (i == numTilesX - 1)
    {
      tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
    }
    else
    {
      float toss = Random.Range(0.0f, 1.0f);
      if (toss < 0.5f)
      {
        tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
      }
      else
      {
        tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
      }
    }

    // Up side tiles
    if (j == numTilesY - 1)
    {
      tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NONE);
    }
    else
    {
      float toss = Random.Range(0.0f, 1.0f);
      if (toss < 0.5f)
      {
        tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.POS);
      }
      else
      {
        tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NEG);
      }
    }

    tile.Apply();
    return tile;
  }

  void CreateJigsawTiles()
  {
    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    numTilesX = baseTexture.width / Tile.tileSize;
    numTilesY = baseTexture.height / Tile.tileSize;

    mTiles = new Tile[numTilesX, numTilesY];
    mTileGameObjects = new GameObject[numTilesX, numTilesY];
    for (int i = 0; i < numTilesX; ++i)
    {
      for (int j = 0; j < numTilesY; ++j)
      {        

        mTiles[i, j] = CreateTile(i, j, baseTexture);

        // Create a game object for the tile.
        mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);

        if (parentForTiles != null)
        {
          mTileGameObjects[i, j].transform.SetParent(parentForTiles);
        }
      }
    }
  }

  IEnumerator Coroutine_CreateJigsawTiles()
  {
    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    numTilesX = baseTexture.width / Tile.tileSize;
    numTilesY = baseTexture.height / Tile.tileSize;

    mTiles = new Tile[numTilesX, numTilesY];
    mTileGameObjects = new GameObject[numTilesX, numTilesY];
    for (int i = 0; i < numTilesX; ++i)
    {
      for (int j = 0; j < numTilesY; ++j)
      {

        mTiles[i, j] = CreateTile(i, j, baseTexture);

        // Create a game object for the tile.
        mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);

        if (parentForTiles != null)
        {
          mTileGameObjects[i, j].transform.SetParent(parentForTiles);
        }
        yield return null;
      }
    }
  }
}
