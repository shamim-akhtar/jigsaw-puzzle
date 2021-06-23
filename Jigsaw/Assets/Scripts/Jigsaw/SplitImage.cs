using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curves;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO.Compression;

public class SplitImage
{
    public string mImageFilename { get; set; }
    public SpriteRenderer mSpriteRenderer { get; set; }
    public Transform TilesParent { get; set; }
    public Material mShadowMaterial { get; set; }

    Vec2[] mCurvyCoords = new Vec2[]
    {
        new Vec2(0, 0),
        new Vec2(35, 15),
        new Vec2(37, 5),
        new Vec2(37, 5),
        new Vec2(40, 0),
        new Vec2(38, -5),
        new Vec2(38, -5),
        new Vec2(20, -20),
        new Vec2(50, -20),
        new Vec2(50, -20),
        new Vec2(80, -20),
        new Vec2(62, -5),
        new Vec2(62, -5),
        new Vec2(60, 0),
        new Vec2(63, 5),
        new Vec2(63, 5),
        new Vec2(65, 15),
        new Vec2(100, 0)
    };

    void CreateSprite()
    {
        Texture2D tex = SpriteUtils.LoadTexture("Images/" + mImageFilename);
        if (tex == null) return;

        int w = tex.width > 140 ? 140 : tex.width;
        int h = tex.height > 140 ? 140 : tex.height;
        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(tex, 0, 0, w, h);

        mSpriteRenderer.sprite = sprite;
    }

    int GetInterpolatedY(List<Vector3> mBezierPoints, int x)
    {
        for(int i = 1; i < mBezierPoints.Count; ++i)
        {
            if(mBezierPoints[i].x >= x)
            {
                float x1 = mBezierPoints[i - 1].x;
                float x2 = mBezierPoints[i].x;

                float y1 = mBezierPoints[i - 1].y;
                float y2 = mBezierPoints[i].y;

                float y = (x - x1) * (y2 - y1) / (x2 - x1) + y1;
                return (int)y;
            }
        }
        return (int)mBezierPoints[mBezierPoints.Count - 1].y;
    }

    public enum Direction
    {
        UP,
        UP_REVERSE,
        RIGHT,
        RIGHT_REVERSE,
        DOWN,
        DOWN_REVERSE,
        LEFT,
        LEFT_REVERSE,
        NONE,
    }

    Direction GetRandomDirection(int side)
    {
        float rand = Random.Range(0.0f, 1.0f);
        switch(side)
        {
            case 0:
                {
                    if (rand < 0.5f) return Direction.UP;
                    else return Direction.UP_REVERSE;
                }
            case 1:
                {
                    if (rand < 0.5f) return Direction.RIGHT;
                    else return Direction.RIGHT_REVERSE;
                }
            case 2:
                {
                    if (rand < 0.5f) return Direction.DOWN;
                    else return Direction.DOWN_REVERSE;
                }
            case 3:
                {
                    if (rand < 0.5f) return Direction.LEFT;
                    else return Direction.LEFT_REVERSE;
                }
        }
        return Direction.UP;
    }

    List<Vector3> mBezierPoints = new List<Vector3>();
    Texture2D mBaseTexture;

    Color trans = new Color(0.0f, 0.0f, 0.0f, 0.0f);

    public GameObject[,] mGameObjects;

    public int mTilesX { get; private set; }
    public int mTilesY { get; private set; }

    void SetupLineRenderer()
    {
        //// show the bezier curve.
        //mBezierCurve.material = new Material(Shader.Find("Sprites/Default"));
        //mBezierCurve.startWidth = 0.1f;
        //mBezierCurve.endWidth = 0.1f;

        //for (int i = 0; i < 100; i++)
        //{
        //    mBezierCurve.SetPosition(i, mBezierPoints[i] + new Vector3(20, 20, 0.0f));
        //}
    }

    void CreateBezierCurve()
    {
        // use bezier curve.
        Bezier bez = new Bezier(mCurvyCoords.OfType<Vec2>().ToList());

        for (int i = 0; i < 100; i++)
        {
            Vec2 bp = bez.ValueAt(i / 100.0f);
            Vector3 p = new Vector3(bp.x, bp.y, 0.0f);

            mBezierPoints.Add(p);
        }
    }

    Texture2D CreateTileTexture(int indx, int indy)
    {
        int w = 140;
        int h = 140;

        Texture2D new_tex = new Texture2D(w, h, TextureFormat.ARGB32, 1, true);

        int startX = indx * 100;
        int startY = indy * 100;
        for (int i = 0; i < 140; ++i)
        {
            for (int j = 0; j < 140; ++j)
            {
                Color color = mBaseTexture.GetPixel(i + startX, j + startY);
                new_tex.SetPixel(i, j, color);
                if (i < 20 && j < 20)
                {
                    new_tex.SetPixel(i, j, trans);
                }
                if (i >= 120 && j < 20)
                {
                    new_tex.SetPixel(i, j, trans);
                }
                if (i >= 120 && j >= 120)
                {
                    new_tex.SetPixel(i, j, trans);
                }
                if (i < 20 && j >= 120)
                {
                    new_tex.SetPixel(i, j, trans);
                }
            }
        }
        return new_tex;
    }

    void CreateSpriteGameObject(int i, int j)
    {
        GameObject obj = new GameObject();
        obj.name = "Tile_" + i.ToString() + "_" + j.ToString();
        SplitTile tile = obj.AddComponent<SplitTile>();

        tile.ShadowMaterial = mShadowMaterial;

        tile.mIndex = new Vector2Int(i, j);
        mGameObjects[i, j] = obj;

        if(TilesParent != null)
        {
            obj.transform.SetParent(TilesParent);
        }

        SpriteRenderer spren = obj.AddComponent<SpriteRenderer>();
        tile.mSpriteRenderer = spren;

        obj.transform.position = new Vector3(i * 100, j * 100, 0.0f);

        // create a new tile texture.
        Texture2D mTileTexture = CreateTileTexture(i, j);

        tile.mDirections[0] = GetRandomDirection(0);
        tile.mDirections[1] = GetRandomDirection(1);
        tile.mDirections[2] = GetRandomDirection(2);
        tile.mDirections[3] = GetRandomDirection(3);

        // check for bottom and left tile.
        if (j > 0)
        {
            SplitTile downTile = mGameObjects[i, j-1].GetComponent<SplitTile>();
            if(downTile.mDirections[0] == Direction.UP)
            {
                tile.mDirections[2] = Direction.DOWN_REVERSE;
            }
            else
            {
                tile.mDirections[2] = Direction.DOWN;
            }
        }

        // check for bottom and left tile.
        if (i > 0)
        {
            SplitTile downTile = mGameObjects[i - 1, j].GetComponent<SplitTile>();
            if (downTile.mDirections[1] == Direction.RIGHT)
            {
                tile.mDirections[3] = Direction.LEFT_REVERSE;
            }
            else
            {
                tile.mDirections[3] = Direction.LEFT;
            }
        }

        if (i == 0)
        {
            tile.mDirections[3] = Direction.NONE;
        }
        if (i == mTilesX - 1)
        {
            tile.mDirections[1] = Direction.NONE;
        }
        if (j == 0)
        {
            tile.mDirections[2] = Direction.NONE;
        }
        if (j == mTilesY - 1)
        {
            tile.mDirections[0] = Direction.NONE;
        }
        for (int d = 0; d < tile.mDirections.Length; ++d)
        {
            if(tile.mDirections[d] != Direction.NONE)
                ApplyBezierMask(mTileTexture, tile.mDirections[d]);
        }

        mTileTexture.Apply();

        // Set the tile texture to the sprite.
        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(mTileTexture, 0, 0, 140, 140);
        spren.sprite = sprite;

        obj.AddComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    public void CreateJigsawTiles()
    {
        // Load the main image.
        Texture2D tex = SpriteUtils.LoadTexture("Images/Jigsaw/" + mImageFilename);
        if (!tex.isReadable)
        {
            Debug.Log("Texture is not readable");
            return;
        }

        mTilesX = tex.width / 100;
        mTilesY = tex.height / 100;

        // add 20 pixel border around.
        Texture2D new_tex = new Texture2D(tex.width + 40, tex.height + 40, TextureFormat.ARGB32, 1, true);
        for(int i = 20; i < tex.width + 20; ++i)
        {
            for (int j = 20; j < tex.height + 20; ++j)
            {
                Color col = tex.GetPixel(i - 20, j - 20);
                col.a = 1.0f;
                new_tex.SetPixel(i, j, col);
            }
        }
        new_tex.Apply();
        mBaseTexture = new_tex;

        // create the bezier curve.
        CreateBezierCurve();

        mGameObjects = new GameObject[mTilesX, mTilesY];
        for (int i = 0; i < mTilesX; ++i)
        {
            for (int j = 0; j < mTilesY; ++j)
            {
                CreateSpriteGameObject(i, j);
            }
        }

        // now make the background image light transparent.
        for (int i = 20; i < tex.width + 20; ++i)
        {
            for (int j = 20; j < tex.height + 20; ++j)
            {
                Color col = tex.GetPixel(i - 20, j - 20);
                col.a = 0.4f;
                new_tex.SetPixel(i, j, col);
            }
        }
        new_tex.Apply();
        mBaseTexture = new_tex;

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(mBaseTexture, 0, 0, mBaseTexture.width, mBaseTexture.height);
        mSpriteRenderer.sprite = sprite;

        //RelocateCamera();
    }

    void RelocateCamera()
    {
        int size = mBaseTexture.height > mBaseTexture.width ? mBaseTexture.height : mBaseTexture.width;
        Camera.main.orthographicSize = 1.2f * size;// 1.2f * mBaseTexture.width;
        Camera.main.transform.position = new Vector3(mBaseTexture.width / 2.0f, mBaseTexture.height / 2.0f, -10.0f);
    }

    void ApplyBezierMask(Texture2D mTileTexture, Direction dir)
    {
        switch(dir)
        {
            case Direction.UP:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = -GetInterpolatedY(mBezierPoints, i);

                        for (int j = 120 + y; j < 140; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                        mTileTexture.SetPixel(i + 20, 120 + y, Color.gray);
                    }
                    break;
                }
            case Direction.UP_REVERSE:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = GetInterpolatedY(mBezierPoints, i);

                        for (int j = 120 + y; j < 140; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                        mTileTexture.SetPixel(i + 20, 120 + y, Color.gray);
                    }
                    break;
                }
            case Direction.RIGHT:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = -GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(120 + x, j + 20, Color.gray);
                        for (int i = 119 + x; i < 140; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
            case Direction.RIGHT_REVERSE:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(120 + x, j + 20, Color.gray);
                        for (int i = 121 + x; i < 140; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
            case Direction.DOWN:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = GetInterpolatedY(mBezierPoints, i);

                        //mTileTexture.SetPixel(i + 20, y + 20, trans);
                        for (int j = 0; j < y + 19; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                    }
                    break;
                }
            case Direction.DOWN_REVERSE:
                {
                    for (int i = 0; i < 100; ++i)
                    {
                        int y = -GetInterpolatedY(mBezierPoints, i);

                        //mTileTexture.SetPixel(i + 20, y + 20, trans);
                        for (int j = 0; j < y + 19; ++j)
                        {
                            mTileTexture.SetPixel(i + 20, j, trans);
                        }
                    }
                    break;
                }
            case Direction.LEFT:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(x + 20, j, trans);
                        for (int i = 0; i < x + 19; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
            case Direction.LEFT_REVERSE:
                {
                    for (int j = 0; j < 100; ++j)
                    {
                        int x = -GetInterpolatedY(mBezierPoints, j);

                        //mTileTexture.SetPixel(x + 20, j + 20, trans);
                        for (int i = 0; i < x + 21; ++i)
                        {
                            mTileTexture.SetPixel(i, j + 20, trans);
                        }
                    }
                    break;
                }
        }
    }

    #region SAVE/LOAD Game
    public void SaveGame()
    {
        BinaryWriter Writer = null;
        string filename = Application.persistentDataPath + "/jigsaw";

        try
        {
            // Create a new stream to write to the file
            Writer = new BinaryWriter(File.OpenWrite(filename));

            // Writer raw data   
            Writer.Write(mImageFilename);
            Writer.Write(mTilesX);
            Writer.Write(mTilesY);

            for (int i = 0; i < mTilesX; ++i)
            {
                for (int j = 0; j < mTilesY; ++j)
                {
                    GameObject obj = mGameObjects[i, j];

                    Writer.Write(mGameObjects[i, j].name);
                    SplitTile tile = obj.GetComponent<SplitTile>();
                    Writer.Write(tile.mIndex.x);
                    Writer.Write(tile.mIndex.y);

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
            Texture2D base_tex = mSpriteRenderer.sprite.texture;

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

    public bool LoadGame()
    {
        string filename = Application.persistentDataPath + "/jigsaw";
        if (File.Exists(filename))
        {
            using (BinaryReader Reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                mImageFilename = Reader.ReadString();
                mTilesX = Reader.ReadInt32();
                mTilesY = Reader.ReadInt32();

                mGameObjects = new GameObject[mTilesX, mTilesY];
                for (int i = 0; i < mTilesX; ++i)
                {
                    for (int j = 0; j < mTilesY; ++j)
                    {
                        GameObject obj = new GameObject();
                        mGameObjects[i, j] = obj;
                        if(TilesParent)
                        {
                            obj.transform.SetParent(TilesParent);
                        }

                        mGameObjects[i, j].name = Reader.ReadString();
                        SplitTile tile = obj.AddComponent<SplitTile>();
                        tile.ShadowMaterial = mShadowMaterial;
                        int tx = Reader.ReadInt32();
                        int ty = Reader.ReadInt32();

                        tile.mIndex = new Vector2Int(tx, ty);

                        float x = Reader.ReadSingle();
                        float y = Reader.ReadSingle();
                        float z = Reader.ReadSingle();
                        obj.transform.position = new Vector3(x, y, z);

                        float rx = Reader.ReadSingle();
                        float ry = Reader.ReadSingle();
                        float rw = Reader.ReadSingle();
                        float rh = Reader.ReadSingle();

                        int length = Reader.ReadInt32();

                        byte[] bytes = new byte[length];
                        Reader.Read(bytes, 0, bytes.Length);

                        SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();

                        int w = 140;
                        int h = 140;

                        Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, 1, true);
                        tex.LoadImage(bytes);
                        tex.Apply();

                        tile.mSpriteRenderer = spriteRenderer;

                        spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(tex, (int)rx, (int)ry, (int)rw, (int)rh);
                        obj.AddComponent<BoxCollider2D>();
                    }
                }

                int base_w = Reader.ReadInt32();
                int base_h = Reader.ReadInt32();

                int base_length = Reader.ReadInt32();

                byte[] base_bytes = new byte[base_length];
                Reader.Read(base_bytes, 0, base_bytes.Length);

                mBaseTexture = new Texture2D(base_w, base_h, TextureFormat.ARGB32, 1, true);
                mBaseTexture.LoadImage(base_bytes);
                mBaseTexture.Apply();

                Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(mBaseTexture, 0, 0, mBaseTexture.width, mBaseTexture.height);
                mSpriteRenderer.sprite = sprite;
            }
            return true;
        }
        return false;
    }
    #endregion
}
