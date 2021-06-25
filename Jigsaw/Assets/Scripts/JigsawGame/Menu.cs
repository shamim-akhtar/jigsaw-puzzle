using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Button BtnHome;
    public FixedButton BtnZoomIn;
    public Button BtnReset;
    public FixedButton BtnZoomOut;
    public Button BtnHint;

    public Text TextTotalTiles;
    public Text TextTilesInPlace;
    public Text TextTime;

    public delegate void DelegateOnClick();
    public DelegateOnClick OnClickHome;
    public DelegateOnClick OnClickZoomIn;
    public DelegateOnClick OnClickReset;
    public DelegateOnClick OnClickZoomOut;
    public DelegateOnClick OnClickHint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BtnZoomIn.Pressed)
        {
            OnClickZoomIn?.Invoke();
        }

        if (BtnZoomOut.Pressed)
        {
            OnClickZoomOut?.Invoke();
        }
    }

    public void SetTotalTiles(int count)
    {
        TextTotalTiles.text = count.ToString();
    }

    public void SetTilesInPlace(int count)
    {
        TextTilesInPlace.text = count.ToString();
    }

    public void OnClickBtnHome()
    {
        OnClickHome?.Invoke();
    }

    public void OnClickBtnReset()
    {
        OnClickReset?.Invoke();
    }

    public void OnClickBtnHint()
    {
        OnClickHint?.Invoke();
    }
}
