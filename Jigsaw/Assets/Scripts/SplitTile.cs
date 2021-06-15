using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitTile : MonoBehaviour
{
    public Vector2Int mIndex;
    public SplitImage.Direction[] mDirections = new SplitImage.Direction[4]; //0 = UP, 1 = RIGHT, 2 = BOTTOM, 3 = LEFT

    private Vector3 offset;

    private Vector3 GetCorrectPosition()
    {
        return new Vector3(mIndex.x * 100.0f, mIndex.y * 100.0f, 0.0f);
    }

    public SpriteRenderer mSpriteRenderer = null;

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
        mSpriteRenderer.sortingOrder = 1;

        offset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }
    void OnMouseUp()
    {
        float distsq = (transform.position - GetCorrectPosition()).sqrMagnitude;
        //Debug.Log("Dist Sqr: " + distsq.ToString());
        if (distsq < 400.0f)
        {
            transform.position = GetCorrectPosition();
        }
        mSpriteRenderer.sortingOrder = 0;
    }
}
