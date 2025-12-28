using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGen : MonoBehaviour
{
  private string imageFilename;
  Sprite mBaseSpriteOpaque;
  Sprite mBaseSpriteTransparent;

  GameObject mGameObjectOpaque;
  GameObject mGameObjectTransparent;

  public float ghostTransparency = 0.1f;

  // Jigsaw tiles creation.
  public int numTileX { get; private set; }
  public int numTileY { get; private set; }

  Tile[,] mTiles = null;
  GameObject[,] mTileGameObjects= null;

  public Transform parentForTiles = null;

  // Access to the menu.
  public Menu menu = null;
  private List<Rect> regions = new List<Rect>();
  private List<Coroutine> activeCoroutines = new List<Coroutine>();

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

  // Start is called before the first frame update
  void Start()
  {
    imageFilename = GameApp.Instance.GetJigsawImageName();

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

    mGameObjectOpaque.gameObject.SetActive(false);

    SetCameraPosition();

    // Create the Jigsaw tiles.
    //CreateJigsawTiles();
    StartCoroutine(Coroutine_CreateJigsawTiles());
  }

  Sprite CreateTransparentView(Texture2D tex)
  {
    Texture2D newTex = new Texture2D(
      tex.width,
      tex.height, 
      TextureFormat.ARGB32, 
      false);

    for(int x = 0; x < newTex.width; x++)
    {
      for(int y = 0; y < newTex.height; y++)
      {
        Color c = tex.GetPixel(x, y);
        if(x > Tile.padding && 
           x < (newTex.width - Tile.padding) &&
           y > Tile.padding && 
           y < (newTex.height - Tile.padding))
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

  void SetCameraPosition()
  {
    Camera.main.transform.position = new Vector3(mBaseSpriteOpaque.texture.width / 2,
      mBaseSpriteOpaque.texture.height / 2, -10.0f);
    //Camera.main.orthographicSize = mBaseSpriteOpaque.texture.width / 2;
    int smaller_value = Mathf.Min(mBaseSpriteOpaque.texture.width, mBaseSpriteOpaque.texture.height);
    Camera.main.orthographicSize = smaller_value * 0.8f;
  }

  public static GameObject CreateGameObjectFromTile(Tile tile)
  {
    GameObject obj = new GameObject();

    obj.name = "TileGameObe_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

    obj.transform.position = new Vector3(tile.xIndex * Tile.tileSize, tile.yIndex * Tile.tileSize, 0.0f);

    SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
    spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
      tile.finalCut,
      0,
      0,
      Tile.padding * 2 + Tile.tileSize,
      Tile.padding * 2 + Tile.tileSize);

    BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

    TileMovement tileMovement = obj.AddComponent<TileMovement>();
    tileMovement.tile = tile;

    return obj;
  }

  void CreateJigsawTiles()
  {
    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    numTileX = baseTexture.width / Tile.tileSize;
    numTileY = baseTexture.height / Tile.tileSize;

    mTiles = new Tile[numTileX, numTileY];
    mTileGameObjects = new GameObject[numTileX, numTileY];

    for(int i = 0; i < numTileX; i++)
    {
      for(int j = 0; j < numTileY; j++)
      {
        mTiles[i, j] = CreateTile(i, j, baseTexture);
        mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
        if(parentForTiles != null)
        {
          mTileGameObjects[i, j].transform.SetParent(parentForTiles);
        }
      }
    }

    // Enable the bottom panel and set the onlcick delegate to the play button.
    menu.SetEnableBottomPanel(true);
    menu.btnPlayOnClick = ShuffleTiles;
  }

  IEnumerator Coroutine_CreateJigsawTiles()
  {
    Texture2D baseTexture = mBaseSpriteOpaque.texture;
    numTileX = baseTexture.width / Tile.tileSize;
    numTileY = baseTexture.height / Tile.tileSize;

    mTiles = new Tile[numTileX, numTileY];
    mTileGameObjects = new GameObject[numTileX, numTileY];

    for (int i = 0; i < numTileX; i++)
    {
      for (int j = 0; j < numTileY; j++)
      {
        mTiles[i, j] = CreateTile(i, j, baseTexture);
        mTileGameObjects[i, j] = CreateGameObjectFromTile(mTiles[i, j]);
        if (parentForTiles != null)
        {
          mTileGameObjects[i, j].transform.SetParent(parentForTiles);
        }

        yield return null;
      }
    }

    // Enable the bottom panel and set the delegate to button play on click.
    menu.SetEnableBottomPanel(true);
    menu.btnPlayOnClick = ShuffleTiles;

  }


  Tile CreateTile(int i, int j, Texture2D baseTexture)
  {
    Tile tile = new Tile(baseTexture);
    tile.xIndex = i;
    tile.yIndex = j;

    // Left side tiles.
    if (i == 0)
    {
      tile.SetCurveType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
    }
    else
    {
      // We have to create a tile that has LEFT direction opposite curve type.
      Tile leftTile = mTiles[i - 1, j];
      Tile.PosNegType rightOp = leftTile.GetCurveType(Tile.Direction.RIGHT);
      tile.SetCurveType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ?
        Tile.PosNegType.POS : Tile.PosNegType.NEG);
    }

    // Bottom side tiles
    if (j == 0)
    {
      tile.SetCurveType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
    }
    else
    {
      Tile downTile = mTiles[i, j - 1];
      Tile.PosNegType upOp = downTile.GetCurveType(Tile.Direction.UP);
      tile.SetCurveType(Tile.Direction.DOWN, upOp == Tile.PosNegType.NEG ?
        Tile.PosNegType.POS : Tile.PosNegType.NEG);
    }

    // Right side tiles.
    if (i == numTileX - 1)
    {
      tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
    }
    else
    {
      float toss = UnityEngine.Random.Range(0f, 1f);
      if(toss < 0.5f)
      {
        tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
      }
      else
      {
        tile.SetCurveType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
      }
    }

    // Up side tile.
    if(j == numTileY - 1)
    {
      tile.SetCurveType(Tile.Direction.UP, Tile.PosNegType.NONE);
    }
    else
    {
      float toss = UnityEngine.Random.Range(0f, 1f);
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


  // Update is called once per frame
  void Update()
  {

  }

  #region Shuffling related codes

  private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
  {
    float elaspedTime = 0.0f;
    Vector3 startingPosition = objectToMove.transform.position;
    while(elaspedTime < seconds)
    {
      objectToMove.transform.position = Vector3.Lerp(
        startingPosition, end, (elaspedTime / seconds));
      elaspedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }
    objectToMove.transform.position = end;
  }

  void Shuffle(GameObject obj)
  {
    if(regions.Count == 0)
    {
      regions.Add(new Rect(-300.0f, -100.0f, 50.0f, numTileY * Tile.tileSize));
      regions.Add(new Rect((numTileX+1) * Tile.tileSize, -100.0f, 50.0f, numTileY * Tile.tileSize));
    }

    int regionIndex = UnityEngine.Random.Range(0, regions.Count);
    float x = UnityEngine.Random.Range(regions[regionIndex].xMin, regions[regionIndex].xMax);
    float y = UnityEngine.Random.Range(regions[regionIndex].yMin, regions[regionIndex].yMax);

    Vector3 pos = new Vector3(x, y, 0.0f);
    Coroutine moveCoroutine = StartCoroutine(Coroutine_MoveOverSeconds(obj, pos, 1.0f));
    activeCoroutines.Add(moveCoroutine);
  }

  IEnumerator Coroutine_Shuffle()
  {
    for(int i = 0; i < numTileX; ++i)
    {
      for(int j = 0; j < numTileY; ++j)
      {
        Shuffle(mTileGameObjects[i, j]);
        yield return null;
      }
    }

    foreach(var item in activeCoroutines)
    {
      if(item != null)
      {
        yield return null;
      }
    }

    OnFinishedShuffling();
  }

  public void ShuffleTiles()
  {
    StartCoroutine(Coroutine_Shuffle());
  }

  void OnFinishedShuffling()
  {
    activeCoroutines.Clear();

    menu.SetEnableBottomPanel(false);
    StartCoroutine(Coroutine_CallAfterDelay(() => menu.SetEnableTopPanel(true), 1.0f));
    GameApp.Instance.TileMovementEnabled = true;

    StartTimer();

    for(int i = 0; i < numTileX; ++i)
    {
      for(int j = 0; j < numTileY; ++j)
      {
        TileMovement tm = mTileGameObjects[i, j].GetComponent<TileMovement>();
        tm.onTileInPlace += OnTileInPlace;
        SpriteRenderer spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();
        Tile.tilesSorting.BringToTop(spriteRenderer);
      }
    }

    menu.SetTotalTiles(numTileX * numTileY);
  }

  IEnumerator Coroutine_CallAfterDelay(System.Action function, float delay)
  {
    yield return new WaitForSeconds(delay);
    function();
  }


  public void StartTimer()
  {
    StartCoroutine(Coroutine_Timer());
  }

  IEnumerator Coroutine_Timer()
  {
    while(true)
    {
      yield return new WaitForSeconds(1.0f);
      GameApp.Instance.SecondsSinceStart += 1;

      menu.SetTimeInSeconds(GameApp.Instance.SecondsSinceStart);
    }
  }

  public void StopTimer()
  {
    StopCoroutine(Coroutine_Timer());
  }

  #endregion

  public void ShowOpaqueImage()
  {
    mGameObjectOpaque.SetActive(true);
  }

  public void HideOpaqueImage()
  {
    mGameObjectOpaque.SetActive(false);
  }

  void OnTileInPlace(TileMovement tm)
  {
    GameApp.Instance.TotalTilesInCorrectPosition += 1;

    tm.enabled = false;
    Destroy(tm);

    SpriteRenderer spriteRenderer = tm.gameObject.GetComponent<SpriteRenderer>();
    Tile.tilesSorting.Remove(spriteRenderer);

    if (GameApp.Instance.TotalTilesInCorrectPosition == mTileGameObjects.Length)
    {
      //Debug.Log("Game completed. We will implement an end screen later");
      menu.SetEnableTopPanel(false);
      menu.SetEnableGameCompletionPanel(true);

      // Reset the values.
      GameApp.Instance.SecondsSinceStart = 0;
      GameApp.Instance.TotalTilesInCorrectPosition = 0;
    }
    menu.SetTilesInPlace(GameApp.Instance.TotalTilesInCorrectPosition);
  }
}
