using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawUtils : MonoBehaviour
{
    public string ImageFilename;

    public List<Vector2> mControlPoints = new List<Vector2>()
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

    private List<Vector2> mBerierCurve = new List<Vector2>();
    private Texture2D mTexture;
    private Color mTransparent = new Color(0.0f, 0.0f, 0.0f, 0.0f);

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
        UP_LEFT,
        UP_RIGHT,
        DOWN_RIGHT,
        DOWN_LEFT,
    }

    Direction GetRandomDirection(int side)
    {
        float rand = UnityEngine.Random.Range(0.0f, 1.0f);
        switch (side)
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

    private SpriteRenderer mSpriteRenderer;
    private List<LineRenderer> mLineRenderers = new List<LineRenderer>();
    private LineRenderer mInnerLine;
    private LineRenderer mOuterLine;
    private LineRenderer mCentreLine;

    #region Util functions
    public static LineRenderer CreateLine()
    {
        GameObject obj = new GameObject();
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.gray;
        lr.endColor = Color.gray;
        lr.startWidth = 1.0f;
        lr.endWidth = 1.0f;
        return lr;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = new GameObject();
        obj.name = ImageFilename;

        obj.transform.SetParent(gameObject.transform);

        mSpriteRenderer = obj.AddComponent<SpriteRenderer>();
        Texture2D tex = SpriteUtils.LoadTexture(ImageFilename);
        if (tex == null)
        {
            Debug.Log("Ccould not load texture from Resources <" + ImageFilename + ">");
            return;
        }

        Texture2D new_tex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, 1, true);
        for (int i = 0; i < tex.width; ++i)
        {
            for (int j = 0; j < tex.height; ++j)
            {
                Color col = tex.GetPixel(i, j);
                col.a = 1.0f;
                new_tex.SetPixel(i, j, col);
            }
        }
        new_tex.Apply();
        mTexture = new_tex;

        Sprite sprite = SpriteUtils.CreateSpriteFromTexture2D(mTexture, 0, 0, mTexture.width, mTexture.height);
        mSpriteRenderer.sprite = sprite;
        mSpriteRenderer.sortingOrder = -1;

        // create one set of bezier points. Later we will
        // transform these points.
        mBerierCurve = BezierCurve.PointList2(mControlPoints, 0.001f);

        // create the inner, outer and centre lines.
        mCentreLine = CreateLine();
        mCentreLine.positionCount = 5;
        mCentreLine.SetPosition(0, new Vector3(20, 20, 0));
        mCentreLine.SetPosition(1, new Vector3(120, 20, 0));
        mCentreLine.SetPosition(2, new Vector3(120, 120, 0));
        mCentreLine.SetPosition(3, new Vector3(20, 120, 0));
        mCentreLine.SetPosition(4, new Vector3(20, 20, 0));

        // 
        InitLineRenderers();
        CreateBezierCurves();
        //CutSprite(mTexture);
    }

    void InitLineRenderers()
    {
        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            LineRenderer lr = CreateLine();
            lr.gameObject.name = "GameObj_LR_" + dir.ToString();
            lr.transform.SetParent(transform);
            mLineRenderers.Add(lr);

            if((int)dir%2 == 0)
            {
                lr.startColor = Color.blue;
                lr.endColor = Color.blue;
            }
            else
            {
                lr.startColor = Color.red;
                lr.endColor = Color.red;
            }
        }
    }

    void TranslatePoints(List<Vector2> iList, List<Vector2> oList, Vector2 offset)
    {
        for(int i = 0; i < iList.Count; ++i)
        {
            oList[i] = iList[i] + offset;
        }
    }

    void InvertY(List<Vector2> iList, List<Vector2> oList)
    {
        for (int i = 0; i < iList.Count; ++i)
        {
            oList[i] = new Vector2(iList[i].x, -iList[i].y);
        }
    }

    void SwapXY(List<Vector2> iList, List<Vector2> oList)
    {
        for (int i = 0; i < iList.Count; ++i)
        {
            oList[i] = new Vector2(iList[i].y, iList[i].x);
        }
    }

    public delegate Vector2 Operate(Vector2 inp);

    List<Vector2> CreateCurve(Direction dir)
    {
        int padding_x = 20;
        int padding_y = 20;
        int sw = 100;
        int sh = 100;

        List<Vector3> lpoints = new List<Vector3>();
        List<Vector2> pts = new List<Vector2>(mBerierCurve);
        switch(dir)
        {
            case Direction.UP:
                {
                    TranslatePoints(pts, pts, new Vector3(padding_x, padding_y + sh, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.UP_REVERSE:
                {
                    InvertY(pts, pts);
                    TranslatePoints(pts, pts, new Vector3(padding_x, padding_y + sh, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.RIGHT:
                {
                    SwapXY(pts, pts);
                    TranslatePoints(pts, pts, new Vector3(padding_x + sw, padding_y, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.RIGHT_REVERSE:
                {
                    InvertY(pts, pts);
                    SwapXY(pts, pts);
                    TranslatePoints(pts, pts, new Vector3(padding_x + sw, padding_y, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.DOWN:
                {
                    InvertY(pts, pts);
                    TranslatePoints(pts, pts, new Vector3(padding_x, padding_y, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.DOWN_REVERSE:
                {
                    TranslatePoints(pts, pts, new Vector3(padding_x, padding_y, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.LEFT:
                {
                    InvertY(pts, pts);
                    SwapXY(pts, pts);
                    TranslatePoints(pts, pts, new Vector3(padding_x, padding_y, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
            case Direction.LEFT_REVERSE:
                {
                    SwapXY(pts, pts);
                    TranslatePoints(pts, pts, new Vector3(padding_x, padding_y, 0));

                    for (int i = 0; i < mBerierCurve.Count; ++i)
                    {
                        lpoints.Add(pts[i]);
                    }
                    break;
                }
        }
        LineRenderer lr = mLineRenderers[(int)dir];
        lr.positionCount = pts.Count;
        lr.SetPositions(lpoints.ToArray());
        return pts;
    }

    int GetInterpolatedY(List<Vector2> mBezierPoints, int x)
    {
        for (int i = 1; i < mBezierPoints.Count; ++i)
        {
            if (mBezierPoints[i].x >= x)
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

    // We will use the orginal (non-translated) points here.
    void CutSprite(Direction dir, Texture2D tex)
    {
        int padding_x = 20;
        int padding_y = 20;
        int sw = 100;
        int sh = 100;
        switch (dir)
        {
            case Direction.UP:
                {
                    for(int x = 0; x < sw; ++x)
                    {
                        int y = GetInterpolatedY(mBerierCurve, x);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + padding_x;
                        int py = y + sh;

                        for(y = py; y < 2*padding_y + sh; ++y)
                        {
                            tex.SetPixel(px, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.UP_REVERSE:
                {
                    for (int x = 0; x < sw; ++x)
                    {
                        int y = -GetInterpolatedY(mBerierCurve, x);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + padding_x;
                        int py = y + sh + padding_y;

                        for (y = py; y < 2 * padding_y + sh; ++y)
                        {
                            tex.SetPixel(px, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.RIGHT:
                {
                    for (int y = 0; y < sh; ++y)
                    {
                        int x = GetInterpolatedY(mBerierCurve, y);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + sw + padding_x;
                        int py = y + padding_y;

                        for (x = px; x < 2 * padding_x + sw; ++x)
                        {
                            tex.SetPixel(x, py, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.RIGHT_REVERSE:
                {
                    for (int y = 0; y < sh; ++y)
                    {
                        int x = -GetInterpolatedY(mBerierCurve, y);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + sw + padding_x;
                        int py = y + padding_y;

                        for (x = px; x < 2 * padding_x + sw; ++x)
                        {
                            tex.SetPixel(x, py, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.DOWN:
                {
                    for (int x = 0; x < sw; ++x)
                    {
                        int y = -GetInterpolatedY(mBerierCurve, x);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + padding_x;
                        int py = y + padding_y;

                        for (y = py; y >=0; --y)
                        {
                            tex.SetPixel(px, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.DOWN_REVERSE:
                {
                    for (int x = 0; x < sw; ++x)
                    {
                        int y = GetInterpolatedY(mBerierCurve, x);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + padding_x;
                        int py = y + padding_y;

                        for (y = py; y >= 0; --y)
                        {
                            tex.SetPixel(px, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.LEFT:
                {
                    for (int y = 0; y < sh; ++y)
                    {
                        int x = -GetInterpolatedY(mBerierCurve, y);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + padding_x;
                        int py = y + padding_y;

                        for (x = px; x >=0; --x)
                        {
                            tex.SetPixel(x, py, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.LEFT_REVERSE:
                {
                    for (int y = 0; y < sh; ++y)
                    {
                        int x = GetInterpolatedY(mBerierCurve, y);

                        // in pixel coordinates (since we start from 20, 20 to 120, 120)
                        int px = x + padding_x;
                        int py = y + padding_y;

                        for (x = px; x >= 0; --x)
                        {
                            tex.SetPixel(x, py, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.UP_RIGHT:
                {
                    for (int x = padding_x + sw; x < 2*padding_x + sw; ++x)
                    {
                        for (int y = padding_y + sh; y < 2*padding_y + sh; ++y)
                        {
                            tex.SetPixel(x, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.DOWN_RIGHT:
                {
                    for (int x = padding_x + sw; x < 2 * padding_x + sw; ++x)
                    {
                        for (int y = 0; y < padding_y; ++y)
                        {
                            tex.SetPixel(x, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.DOWN_LEFT:
                {
                    for (int x = 0; x < padding_x; ++x)
                    {
                        for (int y = 0; y < padding_y; ++y)
                        {
                            tex.SetPixel(x, y, mTransparent);
                        }
                    }
                    break;
                }
            case Direction.UP_LEFT:
                {
                    for (int x = 0; x < padding_x; ++x)
                    {
                        for (int y = padding_y + sh; y < 2* padding_y + sh; ++y)
                        {
                            tex.SetPixel(x, y, mTransparent);
                        }
                    }
                    break;
                }
        }
    }

    void CreateBezierCurves()
    {
        CreateCurve(Direction.UP);
        CreateCurve(Direction.UP_REVERSE);
        CreateCurve(Direction.RIGHT);
        CreateCurve(Direction.RIGHT_REVERSE);
        CreateCurve(Direction.DOWN);
        CreateCurve(Direction.DOWN_REVERSE);
        CreateCurve(Direction.LEFT);
        CreateCurve(Direction.LEFT_REVERSE);
    }

    void CutSprite(Texture2D tex)
    {
        CutSprite(Direction.UP_REVERSE, tex);
        CutSprite(Direction.RIGHT_REVERSE, tex);
        CutSprite(Direction.DOWN_REVERSE, tex);
        CutSprite(Direction.LEFT_REVERSE, tex);
        CutSprite(Direction.UP_RIGHT, tex);
        CutSprite(Direction.DOWN_RIGHT, tex);
        CutSprite(Direction.DOWN_LEFT, tex);
        CutSprite(Direction.UP_LEFT, tex);

        tex.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
