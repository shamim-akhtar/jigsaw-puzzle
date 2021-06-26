using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Puzzle;

public class TileMovement : MonoBehaviour
{
    public Tile tile { get; set; }

    private SpriteRenderer mSpriteRenderer;

    private Vector3 GetCorrectPosition()
    {
        return new Vector3(tile.xIndex * 100.0f, tile.yIndex * 100.0f, 0.0f);
    }

    private Vector3 mOffset = new Vector3(0.0f, 0.0f, 0.0f);

    // Global setting to disable tile movement.
    public static bool TileMovementEnabled { get; set; } = true;

    public delegate void DelegateOnTileInPlace(TileMovement tm);
    public DelegateOnTileInPlace OnTileInPlace;

    // Start is called before the first frame update
    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
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

        if (!TileMovementEnabled || !enabled)
            return;

        // Hit piece. So disable the camera panning.
        CameraMovement.CameraPanning = false;

        Tile.TilesSorting.BringToTop(mSpriteRenderer);
        mOffset = transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));
    }

    void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!TileMovementEnabled || !enabled)
            return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + mOffset;
        transform.position = curPosition;
    }
    void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!TileMovementEnabled || !enabled)
            return;

        float dist = (transform.position - GetCorrectPosition()).magnitude;
        if (dist < 20.0f)
        {
            transform.position = GetCorrectPosition();
            OnTileInPlace?.Invoke(this);
        }

        // Enable back the camera panning.
        CameraMovement.CameraPanning = true;
    }

    void LateUpdate()
    {
    }
}
