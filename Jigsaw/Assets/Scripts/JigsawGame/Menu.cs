using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

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
    GameApp.Instance.mAds.onAdFinish += OnUnityAdsDidFinish;
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

  public float mDurationAdShow = 60.0f;
  public void OnClickBtnHome()
  {

#if (UNITY_ANDROID || UNITY_IOS)
    float currentTime = Time.time;
    bool showAd = false;
    if ((currentTime - JigsawGameData.Instance.mLastAdShowTime) > 
      mDurationAdShow)
    {
      showAd = true;
    }
    Debug.Log("Show ads: " + showAd + ", Duration: " + 
      (currentTime - JigsawGameData.Instance.mLastAdShowTime) + 
      ", mLastAdShowTime: " + 
      JigsawGameData.Instance.mLastAdShowTime);
    if (Advertisement.isInitialized && showAd)
    {
      JigsawGameData.Instance.mLastAdShowTime = Time.time;
      GameApp.Instance.mAds.ShowInterstitialAd();
    }
    else
    {
      FadeSceneLoader.Instance.FadeSceneLoad("JigsawImageSelection");
    }
#else
    FadeSceneLoader.Instance.FadeSceneLoad("JigsawImageSelection");
#endif
  }

  public void OnClickBtnPlay()
  {
    mAudioSource.PlayOneShot(mBtnClickAudio);
    OnClickPlay?.Invoke();
  }


  public void OnClickBtnNext()
  {
    mAudioSource.PlayOneShot(mBtnClickAudio);
#if (UNITY_ANDROID || UNITY_IOS)
    if (Advertisement.isInitialized)
    {
      GameApp.Instance.mAds.ShowInterstitialAd();
    }
    else
    {
      FadeSceneLoader.Instance.FadeSceneLoad("JigsawImageSelection");
    }
#endif
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

  public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
  {
    // Define conditional logic for each ad completion status:
    if (showResult == ShowResult.Finished)
    {
      // Reward the user for watching the ad to completion.
    }
    else if (showResult == ShowResult.Skipped)
    {
      // Do not reward the user for skipping the ad.
    }
    else if (showResult == ShowResult.Failed)
    {
      //Debug.LogWarning(“The ad did not finish due to an error.”);
    }
    FadeSceneLoader.Instance.FadeSceneLoad("JigsawImageSelection");
    //SceneManager.LoadScene("JigsawImageSelection");
  }
}
