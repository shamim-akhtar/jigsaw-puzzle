using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateBezierCurve : MonoBehaviour
{
  public static readonly List<Vector2> templateControlPoints = new List<Vector2>()
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

  // Keep a reference to the point prefab. This prefab 
  // will represent each control point.
  public GameObject PointPrefab;

  // We will also use LineRenderer to show the straight
  // lines connecting these control points as well as
  // the bezier curve based on these control points.
  LineRenderer[] mLineRenderers = null;

  // Keep a list of the points instantiated from the prefab
  // to show the control points.
  List<GameObject> mPointGameObjects = new List<GameObject>();

  // Store the properties of the line.
  public float LineWidth;
  public float LineWidthBezier;
  public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
  public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

  // Let's create a function to create the line renderer.
  private LineRenderer CreateLine()
  {
    GameObject obj = new GameObject();
    LineRenderer lr = obj.AddComponent<LineRenderer>();
    lr.material = new Material(Shader.Find("Sprites/Default"));
    lr.startColor = LineColor;
    lr.endColor = LineColor;
    lr.startWidth = LineWidth;
    lr.endWidth = LineWidth;
    return lr;
  }

  // Start is called before the first frame update
  void Start()
  {
    // Here we will create the actual lines.
    mLineRenderers = new LineRenderer[2];
    mLineRenderers[0] = CreateLine();
    mLineRenderers[1] = CreateLine();

    // Set the name of these lines to distinguish.
    mLineRenderers[0].gameObject.name = "LineRenderer_obj_0";
    mLineRenderers[1].gameObject.name = "LineRenderer_obj_1";

    // Now create the instances of the control points.
    for (int i = 0; i < templateControlPoints.Count; i++)
    {
      GameObject obj = Instantiate(PointPrefab, templateControlPoints[i], Quaternion.identity);
      obj.name = "ControlPoint_" + i.ToString();
      mPointGameObjects.Add(obj);
    }
  }

  // Update is called once per frame
  void Update()
  {
    // We will now draw the lines every frame.
    LineRenderer lineRenderer = mLineRenderers[0];
    LineRenderer curveRenderer = mLineRenderers[1];

    List<Vector2> pts = new List<Vector2>();
    for (int i = 0; i < mPointGameObjects.Count; i++)
    {
      pts.Add(mPointGameObjects[i].transform.position);
    }

    // set the lineRenderer for showing the straight lines between
    // the control points.
    lineRenderer.positionCount = pts.Count;
    for (int i = 0; i < pts.Count; i++)
    {
      lineRenderer.SetPosition(i, pts[i]);
    }

    // We can now see the straight lines connecting the control points.
    // We will now proceed to draw the curve based on the bezier points.
    List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
    curveRenderer.startColor = BezierCurveColor;
    curveRenderer.endColor = BezierCurveColor;
    curveRenderer.positionCount = curve.Count;
    curveRenderer.startWidth = LineWidthBezier;
    curveRenderer.endWidth = LineWidthBezier;

    for (int i = 0; i < curve.Count; i++)
    {
      curveRenderer.SetPosition(i, curve[i]);
    }
  }
}
