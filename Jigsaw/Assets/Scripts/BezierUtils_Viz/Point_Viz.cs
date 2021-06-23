using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Point_Viz : MonoBehaviour
{
    private Vector3 mOffset = Vector3.zero;

    public delegate void OnPointChange(Transform t);
    public OnPointChange mOnDragPoint;
    public OnPointChange mOnDragStart;
    public OnPointChange mOnDragStop;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        mOffset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));


        mOnDragStart?.Invoke(transform);
    }

    void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;
        transform.position = curPosition;

        mOnDragPoint?.Invoke(transform);
    }
    void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        mOnDragStop?.Invoke(transform);
    }

    void LateUpdate()
    {
    }
}
