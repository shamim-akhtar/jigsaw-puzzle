using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public CameraMovement CameraMovement;
    public Button BtnHome;
    public Button BtnPrev;
    public FixedButton BtnZoomIn;
    public Button BtnReset;
    public FixedButton BtnZoomOut;
    public FixedButton BtnHint;

    public Button BtnPlay;
    public Button BtnNext;

    public Text TextTotalTiles;
    public Text TextTilesInPlace;
    public Text TextTime;

    public Text TextWin;

    // Our game controls when the menu is enabled of disabled.
    // Enabled = false means that the UI won't handle
    // inputs.
    static public bool Enabled { get; set; } = true;

    public delegate void DelegateOnClick();
    public DelegateOnClick OnClickHome;
    public DelegateOnClick OnClickPlay;
    public DelegateOnClick OnClickNext;

    public AudioSource mAudioSource;
    public AudioClip mBtnClickAudio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Enabled) return;

        if (BtnZoomIn.Pressed)
        {
            CameraMovement.ZoomIn();
        }

        if (BtnZoomOut.Pressed)
        {
            CameraMovement.ZoomOut();
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

    public void SetTimeInSeconds(double tt)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(tt);

        string time = string.Format("{0:D2}:{1:D2}:{2:D2}",
                        t.Hours,
                        t.Minutes,
                        t.Seconds);

        TextTime.text = time;
    }

    public void OnClickBtnHome()
    {
        //OnClickHome?.Invoke();
        SceneManager.LoadScene("JigsawImageSelection");
    }

    public void OnClickBtnPlay()
    {
        mAudioSource.PlayOneShot(mBtnClickAudio);
        OnClickPlay?.Invoke();
    }

    public void OnClickBtnNext()
    {
        mAudioSource.PlayOneShot(mBtnClickAudio);
        //OnClickNext?.Invoke();
        SceneManager.LoadScene("JigsawImageSelection");
    }

    public void SetActivePlayBtn(bool flag)
    {
        mAudioSource.PlayOneShot(mBtnClickAudio);

        BtnPlay.gameObject.SetActive(flag);
        //BtnNext.gameObject.SetActive(flag);
        BtnPrev.gameObject.SetActive(flag);

        BtnHome.gameObject.SetActive(!flag);
        BtnZoomIn.gameObject.SetActive(!flag);
        BtnReset.gameObject.SetActive(!flag);
        BtnZoomOut.gameObject.SetActive(!flag);
        BtnHint.gameObject.SetActive(!flag);
    }
}
