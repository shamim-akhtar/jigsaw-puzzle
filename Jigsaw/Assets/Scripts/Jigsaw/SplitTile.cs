using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SplitTile : MonoBehaviour
{
    public Vector2Int mIndex;
    public SplitImage.Direction[] mDirections = new SplitImage.Direction[4]; //0 = UP, 1 = RIGHT, 2 = BOTTOM, 3 = LEFT

    private Vector3 offset;

    public Vector2 ShadowOffset = new Vector2(2.0f, -2.0f);
    public Material ShadowMaterial;
    GameObject shadowGameobject;

    private Vector3 GetCorrectPosition()
    {
        return new Vector3(mIndex.x * 100.0f, mIndex.y * 100.0f, 0.0f);
    }

    public SpriteRenderer mSpriteRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        //create a new gameobject to be used as drop shadow
        shadowGameobject = new GameObject("Shadow");

        //create a new SpriteRenderer for Shadow gameobject
        SpriteRenderer shadowSpriteRenderer = shadowGameobject.AddComponent<SpriteRenderer>();

        //set the shadow gameobject's sprite to the original sprite
        shadowSpriteRenderer.sprite = mSpriteRenderer.sprite;
        //set the shadow gameobject's material to the shadow material we created
        shadowSpriteRenderer.material = ShadowMaterial;

        //update the sorting layer of the shadow to always lie behind the sprite
        shadowSpriteRenderer.sortingLayerName = mSpriteRenderer.sortingLayerName;
        shadowSpriteRenderer.sortingOrder = mSpriteRenderer.sortingOrder - 1;
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
        mSpriteRenderer.sortingOrder = 1;

        offset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }

    void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }
    void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        float distsq = (transform.position - GetCorrectPosition()).sqrMagnitude;
        //Debug.Log("Dist Sqr: " + distsq.ToString());
        if (distsq < 400.0f)
        {
            transform.position = GetCorrectPosition();
        }
        mSpriteRenderer.sortingOrder = 0;
    }

    void LateUpdate()
    {
        //update the position and rotation of the sprite's shadow with moving sprite
        shadowGameobject.transform.localPosition = transform.localPosition + (Vector3)ShadowOffset;
        shadowGameobject.transform.localRotation = transform.localRotation;
    }
}
