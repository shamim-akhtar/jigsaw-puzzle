using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle
{
    public class Tile
    {
        #region Static Variables and Functions
        // The padding in pixels for the jigsaw tile.
        // As explained above in the diagram, we are 
        // having each tile of 140 by 140 pixels with
        // 20 pixels padding.
        public static int Padding = 20;

        // The actual size of the tile (minus the padding).
        public static int TileSize = 100;

        // These are the control points for our Bezier curve.
        // These control points do not change are are readonly.
        public static readonly List<Vector2> ControlPoints = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(35, 15),
            new Vector2(47, 13),
            new Vector2(45, 5),
            new Vector2(48, 0),
            new Vector2(25, -5),
            new Vector2(15, -18),
            new Vector2(36, -20),
            new Vector2(64, -20),
            new Vector2(85, -18),
            new Vector2(75, -5),
            new Vector2(52, 0),
            new Vector2(55, 5),
            new Vector2(53, 13),
            new Vector2(65, 15),
            new Vector2(100, 0)
        };

        // The template Bezier curve.
        public static List<Vector2> BezCurve = BezierCurve.PointList2(ControlPoints, 0.001f);


        public static readonly Color TransparentColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        #endregion

        #region Enumerations
        // The 4 directions
        public enum Direction
        {
            UP,
            RIGHT,
            DOWN,
            LEFT,
        }

        // The operations on each of the four directions.
        public enum PosNegType
        {
            POS,
            NEG,
            NONE,
        }
        #endregion

        #region Properties
        public Texture2D FinalCut { get; private set; }
        #endregion

        #region Private variables
        // The original texture used to create this Jigsaw tile.
        // We are not going to change this original texture.
        // Instead, we are going to create a new texture and
        // set the values to either pixels from the original
        // texture or transparent (if the pixel falls outside
        // the curves or straight lines (defined by the enum
        // PosNegType.
        private Texture2D mOriginalTex;

        // The array of PosNegType operations to be performed on 
        // each of the four directions.
        private PosNegType[] mPosNeg = new PosNegType[4]
        {
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE,
            PosNegType.NONE
        };

        // A 2d bollean array that stores whether a particular
        // pixel is visited. This is needed for the flood fill.
        private bool[,] mVisited;

        // A stack needed for the flood fill of the textures.
        private Stack<Vector2Int> mStack = new Stack<Vector2Int>();
        #endregion

        public int xIndex = 0;
        public int yIndex = 0;

        public static TilesSorting TilesSorting { get; set; } = new TilesSorting();

        #region Public Methods

        // The constructor.
        public Tile(Texture2D tex)
        {
            int tileSizeWithPadding = 2 * Padding + TileSize;
            //if (tex.width != tileSizeWithPadding || tex.height != tileSizeWithPadding)
            //{
            //    Debug.Log("Unsupported texture dimension for Jigsaw tile");
            //    return;
            //}

            mOriginalTex = tex;

            // Create a new texture with width and height as Padding + TileSize + Padding.
            FinalCut = new Texture2D(tileSizeWithPadding, tileSizeWithPadding, TextureFormat.ARGB32, false);

            // Initialize the newly create texture with transparent colour.
            for (int i = 0; i < tileSizeWithPadding; ++i)
            {
                for (int j = 0; j < tileSizeWithPadding; ++j)
                {
                    FinalCut.SetPixel(i, j, TransparentColor);
                }
            }
        }

        public void SetPosNegType(Direction dir, PosNegType type)
        {
            mPosNeg[(int)dir] = type;
        }

        public PosNegType GetPosNegType(Direction dir)
        {
            return mPosNeg[(int)dir];
        }

        public void Apply()
        {
            FloodFillInit();
            FloodFill();
            FinalCut.Apply();
        }
        #endregion

        public static GameObject CreateGameObjectFromTile(Tile tile)
        {
            // Create a game object for the tile.
            GameObject obj = new GameObject();

            // Give a name that is recognizable for the GameObject.
            obj.name = "TileGameObj_" + tile.xIndex.ToString() + "_" + tile.yIndex.ToString();

            // Set the position of this GameObject.
            // We will use the xIndex and yIndex to find the actual 
            // position of the tile. We can get this position by multiplying
            // xIndex by TileSize and yIndex by TileSize.
            obj.transform.position = new Vector3(tile.xIndex * TileSize, tile.yIndex * TileSize, 0.0f);

            // Create a SpriteRenderer.
            SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();

            // Set the sorting layer for the tile
            spriteRenderer.sortingLayerName = "Tiles";

            // Set the sprite created with the FinalCut 
            // texture of the tile to the SpriteRenderer
            spriteRenderer.sprite = SpriteUtils.CreateSpriteFromTexture2D(
                tile.FinalCut,
                0,
                0,
                Padding * 2 + TileSize,
                Padding * 2 + TileSize);

            // Add a box colliders so that we can handle 
            // picking/selection of the Tiles.
            BoxCollider2D box = obj.AddComponent<BoxCollider2D>();

            // add the TileMovement script component.
            TileMovement tm = obj.AddComponent<TileMovement>();
            tm.tile = tile;

            Tile.TilesSorting.Add(spriteRenderer);

            return obj;
        }

        #region Helper functions to visualize data
        public static LineRenderer CreateLineRenderer(Color color, float lineWidth = 1.0f)
        {
            GameObject obj = new GameObject();
            
            LineRenderer lr = obj.AddComponent<LineRenderer>();

            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            return lr;
        }

        private Dictionary<(Direction, PosNegType), LineRenderer> mLineRenderers = 
            new Dictionary<(Direction, PosNegType), LineRenderer>();

        // An utility/helper function to show the curve
        // using a LineRenderer.
        public void DrawCurve(Direction dir, PosNegType type, Color color)
        {
            if(!mLineRenderers.ContainsKey((dir, type)))
            {
                mLineRenderers.Add((dir, type), CreateLineRenderer(color));
            }

            LineRenderer lr = mLineRenderers[(dir, type)];
            lr.startColor = color;
            lr.endColor = color;
            lr.gameObject.name = "LineRenderer_" + dir.ToString() + "_" + type.ToString();
            List<Vector2> pts = Tile.CreateCurve(dir, type);

            lr.positionCount = pts.Count;
            for(int i = 0; i < pts.Count; ++i)
            {
                lr.SetPosition(i, pts[i]);
            }
        }
        #endregion

        void Fill(int x, int y)
        {
            Color c = mOriginalTex.GetPixel(x + xIndex * TileSize, y + yIndex * TileSize);
            c.a = 1.0f;
            FinalCut.SetPixel(x, y, c);
        }

        public void FloodFill()
        {
            int width_height = Padding * 2 + TileSize;
            while (mStack.Count > 0)
            {
                //FloodFillStep();
                Vector2Int v = mStack.Pop();

                int xx = v.x;
                int yy = v.y;
                Fill(v.x, v.y);

                // check right.
                int x = xx + 1;
                int y = yy;
                if (x < width_height)
                {
                    Color c = FinalCut.GetPixel(x, y);
                    if (!mVisited[x, y])
                    {
                        mVisited[x, y] = true;
                        mStack.Push(new Vector2Int(x, y));
                    }
                }

                // check left.
                x = xx - 1;
                y = yy;
                if (x >= 0)
                {
                    Color c = FinalCut.GetPixel(x, y);
                    if (!mVisited[x, y])
                    {
                        mVisited[x, y] = true;
                        mStack.Push(new Vector2Int(x, y));
                    }
                }

                // check up.
                x = xx;
                y = yy + 1;
                if (y < width_height)
                {
                    Color c = FinalCut.GetPixel(x, y);
                    if (!mVisited[x, y])
                    {
                        mVisited[x, y] = true;
                        mStack.Push(new Vector2Int(x, y));
                    }
                }

                // check down.
                x = xx;
                y = yy - 1;
                if (y >= 0)
                {
                    Color c = FinalCut.GetPixel(x, y);
                    if (!mVisited[x, y])
                    {
                        mVisited[x, y] = true;
                        mStack.Push(new Vector2Int(x, y));
                    }
                }
            }
        }

        public void FloodFillInit()
        {
            int tileSizeWithPadding = 2 * Padding + TileSize;

            mVisited = new bool[tileSizeWithPadding, tileSizeWithPadding];
            for (int i = 0; i < tileSizeWithPadding; ++i)
            {
                for (int j = 0; j < tileSizeWithPadding; ++j)
                {
                    mVisited[i, j] = false;
                }
            }

            List<Vector2> pts = new List<Vector2>();
            for (int i = 0; i < mPosNeg.Length; ++i)
            {
                pts.AddRange(CreateCurve((Direction)i, mPosNeg[i]));
            }

            // Now we should have a closed curve.
            // To verify check by drawing the pts to a line renderer.
            for (int i = 0; i < pts.Count; ++i)
            {
                mVisited[(int)pts[i].x, (int)pts[i].y] = true;
            }

            // start from center.
            Vector2Int start = new Vector2Int(tileSizeWithPadding / 2, tileSizeWithPadding / 2);

            mVisited[start.x, start.y] = true;
            mStack.Push(start);
        }

        public static List<Vector2> CreateCurve(Tile.Direction dir, PosNegType type)
        {
            int padding_x = Padding;
            int padding_y = Padding;
            int sw = TileSize;
            int sh = TileSize;

            List<Vector2> pts = new List<Vector2>(Tile.BezCurve);
            switch (dir)
            {
                case Tile.Direction.UP:
                    if (type == PosNegType.POS)
                    {
                        TranslatePoints(pts, new Vector3(padding_x, padding_y + sh, 0));
                    }
                    else if (type == PosNegType.NEG)
                    {
                        InvertY(pts);
                        TranslatePoints(pts, new Vector3(padding_x, padding_y + sh, 0));
                    }
                    else if (type == PosNegType.NONE)
                    {
                        pts.Clear();
                        for (int i = 0; i < 100; ++i)
                        {
                            pts.Add(new Vector2(i + padding_x, padding_y + sh));
                        }
                    }
                    break;

            case Tile.Direction.RIGHT:
                if (type == PosNegType.POS)
                {
                    SwapXY(pts);
                    TranslatePoints(pts, new Vector3(padding_x + sw, padding_y, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    InvertY(pts);
                    SwapXY(pts);
                    TranslatePoints(pts, new Vector3(padding_x + sw, padding_y, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    pts.Clear();
                    for (int i = 0; i < 100; ++i)
                    {
                        pts.Add(new Vector2(padding_x + sw, i + padding_y));
                    }
                }
                break;

            case Tile.Direction.DOWN:
                if (type == PosNegType.POS)
                {
                    InvertY(pts);
                    TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    pts.Clear();
                    for (int i = 0; i < 100; ++i)
                    {
                        pts.Add(new Vector2(i + padding_x, padding_y));
                    }
                }
                break;

            case Tile.Direction.LEFT:
                if (type == PosNegType.POS)
                {
                    InvertY(pts);
                    SwapXY(pts);
                    TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                }
                else if (type == PosNegType.NEG)
                {
                    SwapXY(pts);
                    TranslatePoints(pts, new Vector3(padding_x, padding_y, 0));
                }
                else if (type == PosNegType.NONE)
                {
                    pts.Clear();
                    for (int i = 0; i < 100; ++i)
                    {
                        pts.Add(new Vector2(padding_x, i + padding_y));
                    }
                }
                break;
            }
            return pts;
        }

        #region Points transform utilities
        static void TranslatePoints(List<Vector2> iList, Vector2 offset)
        {
            for (int i = 0; i < iList.Count; ++i)
            {
                iList[i] += offset;
            }
        }

        static void InvertY(List<Vector2> iList)
        {
            for (int i = 0; i < iList.Count; ++i)
            {
                iList[i] = new Vector2(iList[i].x, -iList[i].y);
            }
        }

        static void SwapXY(List<Vector2> iList)
        {
            for (int i = 0; i < iList.Count; ++i)
            {
                iList[i] = new Vector2(iList[i].y, iList[i].x);
            }
        }
        #endregion
    }
}