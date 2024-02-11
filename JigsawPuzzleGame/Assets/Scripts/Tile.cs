using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Tile
{
  public enum Direction
  {
    UP,
    RIGHT,
    DOWN,
    LEFT,
  }

  public enum PosNegType
  {
    POS,
    NEG,
    NONE,
  }
  // The array of PosNegType operations to be performed on 
  // each of the four directions.
  private PosNegType[] mPosNeg = new PosNegType[4]
  {
    PosNegType.NONE,
    PosNegType.NONE,
    PosNegType.NONE,
    PosNegType.NONE
  };
  public Vector2Int mOffset = new Vector2Int(20, 20);
  public int tileSize = 100;
  private Dictionary<(Direction, PosNegType), LineRenderer> mLineRenderers = 
    new Dictionary<(Direction, PosNegType), LineRenderer>();

  public static List<Vector2> BezCurve = 
    BezierCurve.PointList2(TemplateBezierCurve.templateControlPoints, 0.001f);

  public Tile()
  { }
  
  public static LineRenderer CreateLineRenderer(UnityEngine.Color color, float lineWidth = 1.0f)
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

  public static void TranslatePoints(List<Vector2> iList, Vector2 offset)
  {
    for (int i = 0; i < iList.Count; ++i)
    {
      iList[i] += offset;
    }
  }
  public static void InvertY(List<Vector2> iList)
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
  public List<Vector2> CreateCurve(Direction dir, PosNegType type)
  {
    int padding_x = mOffset.x;
    int padding_y = mOffset.y;
    int sw = tileSize;
    int sh = tileSize;

    List<Vector2> pts = new List<Vector2>(BezCurve);
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

  public void DrawCurve(Direction dir, PosNegType type, UnityEngine.Color color)
  {
    if (!mLineRenderers.ContainsKey((dir, type)))
    {
      mLineRenderers.Add((dir, type), CreateLineRenderer(color));
    }

    LineRenderer lr = mLineRenderers[(dir, type)];
    lr.startColor = color;
    lr.endColor = color;
    lr.gameObject.name = "LineRenderer_" + dir.ToString() + "_" + type.ToString();
    List<Vector2> pts = CreateCurve(dir, type);

    lr.positionCount = pts.Count;
    for (int i = 0; i < pts.Count; ++i)
    {
      lr.SetPosition(i, pts[i]);
    }
  }
}
