using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO.Compression;

public class BoardGen : MonoBehaviour
{
    public string ImageFilename;
    public Transform ParentForTiles;

    public bool LoadingFinished { get; set; } = false;

    // The opaque sprite. 
    Sprite mBaseSpriteOpaque;

    // The transparent (or Ghost sprite)
    Sprite mBaseSpriteTransparent;

    // The game object that holds the opaque sprite.
    // This should be SetActive to false by default.
    GameObject mGameObjectOpaque;

    // The game object that holds the transparent sprite.
    GameObject mGameObjectTransparent;

    Sprite LoadBaseTexture()
    {
        Texture2D tex = SpriteUtils.LoadTexture(ImageFilename);
        if (!tex.isReadable)
        {
            Debug.Log("Error: Texture is not readable");
            return null;
        }

        if(tex.width % Tile.TileSize != 0 || tex.height % Tile.TileSize != 0)
        {
            Debug.Log("Error: Image must be of size that is multiple of <" + Tile.TileSize + ">");
            return null;
        }

        // Add padding to the image.
        Texture2D newTex = new Texture2D(
            tex.width + Tile.Padding * 2, 
            tex.height + Tile.Padding * 2, 
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
                newTex.SetPixel(x + Tile.Padding, y + Tile.Padding, color);
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

    Sprite CreateTransparentView()
    {
        Texture2D tex = mBaseSpriteOpaque.texture;

        // Add padding to the image.
        Texture2D newTex = new Texture2D(
            tex.width,
            tex.height,
            TextureFormat.ARGB32,
            false);

        //for (int x = Tile.Padding; x < Tile.Padding + Tile.TileSize; ++x)
        for (int x = 0; x < newTex.width; ++x)
        {
            //for (int y = Tile.Padding; y < Tile.Padding + Tile.TileSize; ++y)
            for (int y = 0; y < newTex.height; ++y)
            {
                Color c = tex.GetPixel(x, y);
                if (x > Tile.Padding && x < (newTex.width - Tile.Padding) && y > Tile.Padding && y < newTex.height - Tile.Padding)
                {
                    c.a = 0.2f;
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

    #region
    public void Save()
    {
        string imageFilename = JigsawGameData.Instance.GetImageFilename();
        BinaryWriter Writer = null;
        string filename = Application.persistentDataPath + "/" + imageFilename;

        try
        {
            // Create a new stream to write to the file
            Writer = new BinaryWriter(File.OpenWrite(filename));

            ImageMetaData data = JigsawGameData.Instance.GetCurrentImageData();
            // get the time and save it.
            //long dateTime = System.DateTime.Now.ToBinary();
            Writer.Write(data.secondsSinceStart);

            // Writer raw data   
            Writer.Write(imageFilename);
            Writer.Write(NumTilesX);
            Writer.Write(NumTilesY);

            for (int i = 0; i < NumTilesX; ++i)
            {
                for (int j = 0; j < NumTilesY; ++j)
                {
                    GameObject obj = mTileGameObjects[i, j];

                    //Writer.Write(mTileGameObjects[i, j].name);
                    Tile tile = mTiles[i, j];
                    Writer.Write(tile.xIndex);
                    Writer.Write(tile.yIndex);

                    Writer.Write(obj.transform.position.x);
                    Writer.Write(obj.transform.position.y);
                    Writer.Write(obj.transform.position.z);

                    SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                    Writer.Write(spriteRenderer.sprite.rect.x);
                    Writer.Write(spriteRenderer.sprite.rect.y);
                    Writer.Write(spriteRenderer.sprite.rect.width);
                    Writer.Write(spriteRenderer.sprite.rect.height);

                    Texture2D tex = spriteRenderer.sprite.texture;

                    byte[] bytes = tex.EncodeToPNG();

                    Writer.Write(bytes.Length);
                    Writer.Write(bytes, 0, bytes.Length);
                }
            }
            Texture2D base_tex = mBaseSpriteOpaque.texture;

            byte[] base_bytes = base_tex.EncodeToPNG();

            Writer.Write(base_tex.width);
            Writer.Write(base_tex.height);
            Writer.Write(base_bytes.Length);
            Writer.Write(base_bytes, 0, base_bytes.Length);

            Writer.Close();
        }
        catch (SerializationException e)
        {
            Debug.Log("Failed to save jigsaw game. Reason: " + e.Message);
            throw;
        }
    }

    public bool Load()
    {
        string imageFilename = JigsawGameData.Instance.GetImageFilename();
        string filename = Application.persistentDataPath + "/" + imageFilename;
        if (File.Exists(filename))
        {
            using (BinaryReader Reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                ImageMetaData data = JigsawGameData.Instance.GetCurrentImageData();
                data.secondsSinceStart = Reader.ReadDouble();

                string ifilename = Reader.ReadString();
                NumTilesX = Reader.ReadInt32();
                NumTilesY = Reader.ReadInt32();

                mTileGameObjects = new GameObject[NumTilesX, NumTilesY];
                mTiles = new Tile[NumTilesX, NumTilesY];

                for (int i = 0; i < NumTilesX; ++i)
                {
                    for (int j = 0; j < NumTilesY; ++j)
                    {
                        int tx = Reader.ReadInt32();
                        int ty = Reader.ReadInt32();

                        float x = Reader.ReadSingle();
                        float y = Reader.ReadSingle();
                        float z = Reader.ReadSingle();

                        float rx = Reader.ReadSingle();
                        float ry = Reader.ReadSingle();
                        float rw = Reader.ReadSingle();
                        float rh = Reader.ReadSingle();

                        int length = Reader.ReadInt32();

                        byte[] bytes = new byte[length];
                        Reader.Read(bytes, 0, bytes.Length);

                        Texture2D tex = new Texture2D((int)rw, (int)rh, TextureFormat.ARGB32, 1, true);
                        tex.LoadImage(bytes);
                        tex.Apply();

                        Tile tile = new Tile();
                        tile.xIndex = tx;
                        tile.yIndex = ty;
                        tile.SetFinalCut(tex);

                        mTiles[i, j] = tile;

                        // Create a game object for the tile.
                        mTileGameObjects[i, j] = Tile.CreateGameObjectFromTile(tile);

                        if (ParentForTiles != null)
                        {
                            mTileGameObjects[i, j].transform.SetParent(ParentForTiles);
                        }
                        mTileGameObjects[i, j].transform.position = new Vector3(x, y, z);
                    }
                }

                int base_w = Reader.ReadInt32();
                int base_h = Reader.ReadInt32();

                int base_length = Reader.ReadInt32();

                byte[] base_bytes = new byte[base_length];
                Reader.Read(base_bytes, 0, base_bytes.Length);

                Texture2D mBaseTexture = new Texture2D(base_w, base_h, TextureFormat.ARGB32, 1, true);
                mBaseTexture.LoadImage(base_bytes);
                mBaseTexture.Apply();

                mBaseSpriteOpaque = SpriteUtils.CreateSpriteFromTexture2D(
                    mBaseTexture,
                    0,
                    0,
                    mBaseTexture.width,
                    mBaseTexture.height);

                mGameObjectOpaque = new GameObject();
                mGameObjectOpaque.name = ImageFilename + "_Opaque";
                mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
                mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

                mBaseSpriteTransparent = CreateTransparentView();
                mGameObjectTransparent = new GameObject();
                mGameObjectTransparent.name = ImageFilename + "_Transparent";
                mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
                mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";
            }
            return true;
        }
        return false;
    }
    #endregion

    #region Coroutines to load create the board.
    public void CreateJigsawBoardUsingCoroutines()
    {
        mBaseSpriteOpaque = LoadBaseTexture();

        NumTilesX = mBaseSpriteOpaque.texture.width / Tile.TileSize;
        NumTilesY = mBaseSpriteOpaque.texture.height / Tile.TileSize;

        mGameObjectOpaque = new GameObject();
        mGameObjectOpaque.name = ImageFilename + "_Opaque";
        mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
        mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

        mBaseSpriteTransparent = CreateTransparentView();
        mGameObjectTransparent = new GameObject();
        mGameObjectTransparent.name = ImageFilename + "_Transparent";
        mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
        mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

        StartCoroutine(Coroutine_CreateJigsawBoard());
    }

    IEnumerator Coroutine_CreateJigsawBoard()
    {
        yield return StartCoroutine(Coroutine_CreateJigsawTiles());

        // Hide the mBaseSpriteOpaque game object.
        mGameObjectOpaque.gameObject.SetActive(false);

        LoadingFinished = true;
    }


    IEnumerator Coroutine_CreateJigsawTiles()
    {
        Texture2D baseTexture = mBaseSpriteOpaque.texture;
        NumTilesX = baseTexture.width / Tile.TileSize;
        NumTilesY = baseTexture.height / Tile.TileSize;

        mTiles = new Tile[NumTilesX, NumTilesY];
        mTileGameObjects = new GameObject[NumTilesX, NumTilesY];
        for (int i = 0; i < NumTilesX; ++i)
        {
            for (int j = 0; j < NumTilesY; ++j)
            {
                Tile tile = new Tile(baseTexture);
                tile.xIndex = i;
                tile.yIndex = j;

                // Left side tiles
                if (i == 0)
                {
                    tile.SetPosNegType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
                }
                else
                {
                    // We have to create a tile that has LEFT direction opposite operation 
                    // of the tile on the left's RIGHT direction operation.
                    Tile leftTile = mTiles[i - 1, j];
                    Tile.PosNegType rightOp = leftTile.GetPosNegType(Tile.Direction.RIGHT);
                    tile.SetPosNegType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
                }

                // Bottom side tiles
                if (j == 0)
                {
                    tile.SetPosNegType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
                }
                else
                {
                    // We have to create a tile that has LEFT direction opposite operation 
                    // of the tile on the left's RIGHT direction operation.
                    Tile downTile = mTiles[i, j - 1];
                    Tile.PosNegType rightOp = downTile.GetPosNegType(Tile.Direction.UP);
                    tile.SetPosNegType(Tile.Direction.DOWN, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
                }

                // Right side tiles
                if (i == NumTilesX - 1)
                {
                    tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
                }
                else
                {
                    float toss = Random.Range(0.0f, 1.0f);
                    if (toss < 0.5f)
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
                    }
                    else
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
                    }
                }

                // Up side tiles
                if (j == NumTilesY - 1)
                {
                    tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NONE);
                }
                else
                {
                    float toss = Random.Range(0.0f, 1.0f);
                    if (toss < 0.5f)
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.POS);
                    }
                    else
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NEG);
                    }
                }

                tile.Apply();

                mTiles[i, j] = tile;

                // Create a game object for the tile.
                mTileGameObjects[i, j] = Tile.CreateGameObjectFromTile(tile);

                if (ParentForTiles != null)
                {
                    mTileGameObjects[i, j].transform.SetParent(ParentForTiles);
                }
            }
            yield return null;
        }
    }
    #endregion

    public void CreateJigsawBoard()
    {
        mBaseSpriteOpaque = LoadBaseTexture();

        Texture2D baseTexture = mBaseSpriteOpaque.texture;
        NumTilesX = baseTexture.width / Tile.TileSize;
        NumTilesY = baseTexture.height / Tile.TileSize;

        mGameObjectOpaque = new GameObject();
        mGameObjectOpaque.name = ImageFilename + "_Opaque";
        mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
        mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

        mBaseSpriteTransparent = CreateTransparentView();
        mGameObjectTransparent = new GameObject();
        mGameObjectTransparent.name = ImageFilename + "_Transparent";
        mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
        mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";

        CreateJigsawTiles();

        // Hide the mBaseSpriteOpaque game object.
        mGameObjectOpaque.gameObject.SetActive(false);
    }

    protected void Start()
    {
        CreateJigsawBoard();
    }

    public int NumTilesX { get; private set; }
    public int NumTilesY { get; private set; }

    protected Tile[,] mTiles = null;
    public GameObject[,] mTileGameObjects = null;

    void CreateJigsawTiles()
    {
        Texture2D baseTexture = mBaseSpriteOpaque.texture;

        mTiles = new Tile[NumTilesX, NumTilesY];
        mTileGameObjects = new GameObject[NumTilesX, NumTilesY];
        for (int i = 0; i < NumTilesX; ++i)
        {
            for(int j = 0; j < NumTilesY; ++j)
            {
                Tile tile = new Tile(baseTexture);
                tile.xIndex = i;
                tile.yIndex = j;

                // Left side tiles
                if (i == 0)
                {
                    tile.SetPosNegType(Tile.Direction.LEFT, Tile.PosNegType.NONE);
                }
                else
                {
                    // We have to create a tile that has LEFT direction opposite operation 
                    // of the tile on the left's RIGHT direction operation.
                    Tile leftTile = mTiles[i - 1, j];
                    Tile.PosNegType rightOp = leftTile.GetPosNegType(Tile.Direction.RIGHT);
                    tile.SetPosNegType(Tile.Direction.LEFT, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
                }

                // Bottom side tiles
                if (j == 0)
                {
                    tile.SetPosNegType(Tile.Direction.DOWN, Tile.PosNegType.NONE);
                }
                else
                {
                    // We have to create a tile that has LEFT direction opposite operation 
                    // of the tile on the left's RIGHT direction operation.
                    Tile downTile = mTiles[i, j-1];
                    Tile.PosNegType rightOp = downTile.GetPosNegType(Tile.Direction.UP);
                    tile.SetPosNegType(Tile.Direction.DOWN, rightOp == Tile.PosNegType.NEG ? Tile.PosNegType.POS : Tile.PosNegType.NEG);
                }

                // Right side tiles
                if (i == NumTilesX - 1)
                {
                    tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NONE);
                }
                else
                {
                    float toss = Random.Range(0.0f, 1.0f);
                    if(toss < 0.5f)
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.POS);
                    }
                    else
                    {
                        tile.SetPosNegType(Tile.Direction.RIGHT, Tile.PosNegType.NEG);
                    }
                }

                // Up side tiles
                if (j == NumTilesY - 1)
                {
                    tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NONE);
                }
                else
                {
                    float toss = Random.Range(0.0f, 1.0f);
                    if (toss < 0.5f)
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.POS);
                    }
                    else
                    {
                        tile.SetPosNegType(Tile.Direction.UP, Tile.PosNegType.NEG);
                    }
                }

                tile.Apply();

                mTiles[i, j] = tile;

                // Create a game object for the tile.
                mTileGameObjects[i, j] = Tile.CreateGameObjectFromTile(tile);

                if (ParentForTiles != null)
                {
                    mTileGameObjects[i, j].transform.SetParent(ParentForTiles);
                }
            }
        }
    }

    #region Other public functions
    public void ShowOpaqueImage(bool flag)
    {
        mGameObjectOpaque.SetActive(flag);
    }
    #endregion

    private void OnDestroy()
    {
        Save();
        JigsawGameData.Instance.SaveMetaData();
    }
}
