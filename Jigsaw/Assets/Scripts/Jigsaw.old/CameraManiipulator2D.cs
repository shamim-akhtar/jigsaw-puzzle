using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a 2d camera manipulator for 2d scenes.
/// </summary>

public class CameraManiipulator2D : MonoBehaviour
{
    public Camera mCamera;
    public FixedTouchField mTouchFieldUpDown;
    public FixedTouchField mTouchFieldLeftRight;

    public float mPanSpeed = 10.0f;

    private float mCameraSizeMax;// = 100.0f;
    private float mCameraSizeMin = 250.0f;

    public bool PanMode { get; set; } = true;
    private Vector3 mOriginalPosition;

    #region UI variables
    public Slider mSliderZoom;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        mCameraSizeMax = mCamera.orthographicSize;
        mOriginalPosition = mCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (mTouchFieldUpDown == null || mTouchFieldLeftRight == null) return;
        if(PanMode)
        {
            float x = mTouchFieldLeftRight.TouchDist.x * Time.deltaTime * mPanSpeed/* * (1.1f - mSliderZoom.value)*/;
            float y = mTouchFieldUpDown.TouchDist.y * Time.deltaTime * mPanSpeed/* * (1.1f - mSliderZoom.value)*/;

            mCamera.transform.position -= new Vector3(x, y, 0.0f);
        }
    }

    public void Zoom(float value)
    {
        mCamera.orthographicSize = mCameraSizeMax - value * (mCameraSizeMax - mCameraSizeMin);
    }

    public void Pan()
    {

    }

    public void ResetCameraView()
    {
        mCamera.transform.position = mOriginalPosition;
        mCamera.orthographicSize = mCameraSizeMax;
        mSliderZoom.value = 0.0f;
    }

    #region UI functions
    public void OnSliderChanged()
    {
        Zoom(mSliderZoom.value);
    }
    #endregion
}
