using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public Camera mCamera;
    private Vector3 mDragPos;
    private Vector3 mOriginalPosition;

    private float mCameraSizeMax;// = 100.0f;
    public float mCameraSizeMin = 150.0f;
    private float mSliderZoomValue = 0.0f;

    public Menu menu;

    public static bool CameraPanning { get; set; } = true;

    void Start()
    {
        mCameraSizeMax = mCamera.orthographicSize;
        mOriginalPosition = mCamera.transform.position;

        menu.OnClickReset += ResetCameraView;
        menu.OnClickZoomIn += ZoomIn;
        menu.OnClickZoomOut += ZoomOut;
    }

    public void RePositionCamera(int numTilesX, int numTilesY)
    {
        mCamera.orthographicSize = numTilesX < numTilesY ? numTilesX * 100 : numTilesY * 100;
        mCamera.transform.position = new Vector3((numTilesX * 100 + 40) / 2, (numTilesY * 100 + 40) / 2, -1000.0f);

        mCameraSizeMax = mCamera.orthographicSize;
        mOriginalPosition = mCamera.transform.position;
    }

    void Update()
    {
        // Camera panning is disabled when a tile is selected.
        if (!CameraPanning) return;
        if (EventSystem.current.IsPointerOverGameObject() || enabled == false)
        {
            return;
        }

        // save position is worldspace.
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

    public void Zoom(float value)
    {
        mSliderZoomValue = value;
        mSliderZoomValue = Mathf.Clamp01(mSliderZoomValue);
        //mSliderZoom.value = mSliderZoomValue;

        mCamera.orthographicSize = mCameraSizeMax - mSliderZoomValue * (mCameraSizeMax - mCameraSizeMin);
    }

    public void ResetCameraView()
    {
        mCamera.transform.position = mOriginalPosition;
        mCamera.orthographicSize = mCameraSizeMax;
        mSliderZoomValue = 0.0f;
        //mSliderZoom.value = 0.0f;
    }

    public void ZoomIn()
    {
        Zoom(mSliderZoomValue + 0.01f);
    }

    public void ZoomOut()
    {
        Zoom(mSliderZoomValue - 0.01f);
    }
}
