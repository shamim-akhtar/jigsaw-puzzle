using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public float CameraSizeMin = 150.0f;
    public static bool CameraPanning { get; set; } = true;

    private Vector3 mDragPos;
    private Vector3 mOriginalPosition;

    private float mCameraSizeMax;
    private float mZoomFactor = 0.0f;
    private Camera mCamera;

    void Start()
    {
        mCamera = Camera.main;
        mCameraSizeMax = mCamera.orthographicSize;
        mOriginalPosition = mCamera.transform.position;
    }

    public void RePositionCamera(int numTilesX, int numTilesY)
    {
        //float size = numTilesX < numTilesY ?
        //    (1 + numTilesX) * 100 : (1 + numTilesY) * 100;
        float size = (1 + numTilesX) * 100;
        if(Screen.height < Screen.width)
        {
            size = (1 + numTilesY) * 100;
        }
        // We set the size of the camera. 
        // You can implement your own way of doing this.
        mCamera.orthographicSize = size + 60;

        // Set the position of the camera to be at the
        // centre of the board.
        mCamera.transform.position = new Vector3(
            (numTilesX * 100 + 40) / 2, 
            (numTilesY * 100 + 40) / 2 - 120, 
            -1000.0f);

        mCameraSizeMax = mCamera.orthographicSize;
        mOriginalPosition = mCamera.transform.position;
    }

    void Update()
    {
        // Camera panning is disabled when a tile is selected.
        if (!CameraPanning) return;

        // We also check if the pointer is not on UI item
        // or is disabled.
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }

        // Save the position in worldspace.
        if (Input.GetMouseButtonDown(0))
        {
            mDragPos = mCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0))
        {
            Vector3 diff = mDragPos - mCamera.ScreenToWorldPoint(Input.mousePosition);
            diff.z = 0.0f;
            mCamera.transform.position += diff;
        }
    }

    public void ResetCameraView()
    {
        mCamera.transform.position = mOriginalPosition;
        mCamera.orthographicSize = mCameraSizeMax;
        mZoomFactor = 0.0f;
        //mSliderZoom.value = 0.0f;
    }

    public void Zoom(float value)
    {
        mZoomFactor = value;
        mZoomFactor = Mathf.Clamp01(mZoomFactor);
        //mSliderZoom.value = mZoomFactor;

        mCamera.orthographicSize = mCameraSizeMax - 
            mZoomFactor * (mCameraSizeMax - CameraSizeMin);
    }

    public void ZoomIn()
    {
        Zoom(mZoomFactor + 0.01f);
    }

    public void ZoomOut()
    {
        Zoom(mZoomFactor - 0.01f);
    }
}
