using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Point_Viz : MonoBehaviour
{
  // The offset when we want to click and drag a point.
  private Vector3 mOffset = Vector3.zero;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  private void OnMouseDown()
  {
    if(EventSystem.current.IsPointerOverGameObject())
    {
      return;
    }

    mOffset = transform.position - Camera.main.ScreenToWorldPoint(
      new Vector3(Input.mousePosition.x,
                  Input.mousePosition.y, 0.0f));

  }

  private void OnMouseDrag()
  {
    if (EventSystem.current.IsPointerOverGameObject())
    {
      return;
    }

    Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
    Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;
    transform.position = curPosition;

  }

  private void OnMouseUp()
  {
    if (EventSystem.current.IsPointerOverGameObject())
    {
      return;
    }

  }
}
