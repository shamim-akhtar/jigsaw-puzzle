using UnityEngine;
using Puzzle;
using System.IO;
using System.Runtime.Serialization;

public class JigsawBoard : MonoBehaviour
{
    public Transform ParentForTiles;
    // the image filename
    public string mImageFilename = "default";

    #region Private variables

    // The opaque sprite. 
    Sprite mBaseSpriteOpaque;

    // The transparent (or Ghost sprite)
    Sprite mBaseSpriteTransparent;

    // The game object that holds the opaque sprite.
    // This should be SetActive to false by default.
    GameObject mGameObjectOpaque;

    // The game object that holds the transparent sprite.
    GameObject mGameObjectTransparent;
    #endregion

    public int NumTilesX { get; private set; }
    public int NumTilesY { get; private set; }

    public Tile[,] Tiles { get; private set; } = null;
    public GameObject[,] TileGameObjects { get; private set; } = null;

    private void Start()
    {
        if(ParentForTiles == null)
        {
            ParentForTiles = transform;
        }
    }

    #region Jigsaw raw (Create from image)
    void CreateOpaqueSprite()
    {
        Texture2D tex = SpriteUtils.LoadTexture(mImageFilename);
        if (!tex.isReadable)
        {
            Debug.Log("Error: Texture is not readable");
            return;
        }

        if (tex.width % Tile.TileSize != 0 || tex.height % Tile.TileSize != 0)
        {
            Debug.Log("Error: Image must be of size that is multiple of <" + Tile.TileSize + ">");
            return;
        }

        NumTilesX = tex.width / Tile.TileSize;
        NumTilesY = tex.height / Tile.TileSize;

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

        mBaseSpriteOpaque = SpriteUtils.CreateSpriteFromTexture2D(
            newTex,
            0,
            0,
            newTex.width,
            newTex.height);

        mGameObjectOpaque = new GameObject();
        mGameObjectOpaque.name = mImageFilename + "_Opaque";
        mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
        mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";
    }

    void CreateTransparentSprite()
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

        mBaseSpriteTransparent = SpriteUtils.CreateSpriteFromTexture2D(
            newTex,
            0,
            0,
            newTex.width,
            newTex.height);

        mGameObjectTransparent = new GameObject();
        mGameObjectTransparent.name = mImageFilename + "_Transparent";
        mGameObjectTransparent.AddComponent<SpriteRenderer>().sprite = mBaseSpriteTransparent;
        mGameObjectTransparent.GetComponent<SpriteRenderer>().sortingLayerName = "Transparent";
    }

    void CreateJigsawTiles()
    {
        Texture2D baseTexture = mBaseSpriteOpaque.texture;

        Tiles = new Tile[NumTilesX, NumTilesY];
        TileGameObjects = new GameObject[NumTilesX, NumTilesY];
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
                    Tile leftTile = Tiles[i - 1, j];
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
                    Tile downTile = Tiles[i, j - 1];
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

                Tiles[i, j] = tile;

                // Create a game object for the tile.
                TileGameObjects[i, j] = Tile.CreateGameObjectFromTile(tile);

                if (ParentForTiles != null)
                {
                    TileGameObjects[i, j].transform.SetParent(ParentForTiles);
                }
            }
        }
    }

    #endregion

    // Creates a jigsaw board from an image.
    public void CreateJigsawBoard()
    {
        CreateOpaqueSprite();
        CreateTransparentSprite();
        CreateJigsawTiles();
    }
    // Loads a Jigsaw borad from a file.
    // The filename is same as the image filename.
    public void LoadJigsawBoard()
    {
        Load();
    }

    #region Save and Load of Jigsaw Tiles.
    public void Save()
    {
        BinaryWriter Writer = null;
        string filename = Application.persistentDataPath + "/" + mImageFilename + ".jigsaw";

        try
        {
            // Create a new stream to write to the file
            Writer = new BinaryWriter(File.OpenWrite(filename));

            // Write the version number.
            Writer.Write(Application.version);

            // Write the number of tiles.
            Writer.Write(NumTilesX);
            Writer.Write(NumTilesY);

            for (int i = 0; i < NumTilesX; ++i)
            {
                for (int j = 0; j < NumTilesY; ++j)
                {
                    GameObject obj = TileGameObjects[i, j];

                    Tile tile = Tiles[i, j];
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
        string filename = Application.persistentDataPath + "/" + mImageFilename + ".jigsaw";
        if (File.Exists(filename))
        {
            using (BinaryReader Reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                string version = Reader.ReadString();

                if (version != Application.version)
                {
                    Debug.Log("Can't load jigsaw file. Version mismatch.");
                    return false;
                }

                NumTilesX = Reader.ReadInt32();
                NumTilesY = Reader.ReadInt32();

                TileGameObjects = new GameObject[NumTilesX, NumTilesY];
                Tiles = new Tile[NumTilesX, NumTilesY];

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

                        Tiles[i, j] = tile;

                        // Create a game object for the tile.
                        TileGameObjects[i, j] = Tile.CreateGameObjectFromTile(tile);

                        if (ParentForTiles != null)
                        {
                            TileGameObjects[i, j].transform.SetParent(ParentForTiles);
                        }
                        TileGameObjects[i, j].transform.position = new Vector3(x, y, z);
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
                mGameObjectOpaque.name = mImageFilename + "_Opaque";
                mGameObjectOpaque.AddComponent<SpriteRenderer>().sprite = mBaseSpriteOpaque;
                mGameObjectOpaque.GetComponent<SpriteRenderer>().sortingLayerName = "Opaque";

                CreateTransparentSprite();
            }
            return true;
        }
        return false;
    }
    #endregion
}
