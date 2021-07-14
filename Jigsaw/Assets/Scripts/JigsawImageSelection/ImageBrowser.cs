using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.EventSystems;

public class ImageBrowser : MonoBehaviour
{
    public Image mImage;
    public Text mName;
    public Text mCredit;
    public Text mStatus;
    public Text mTiles;
    public Text mTimeSpent;

    public Button mBtnShowAll;
    public Button mBtnCompleted;
    public Button mBtnPlaying;
    int sw = 2;

    public Menu menu;

    void Start()
    {
        StartCoroutine(Coroutine_WaitUntilMetadataLoaded());
    }

    void Update()
    {
        if (sw == 0) mBtnPlaying.Select();
        if (sw == 1) mBtnCompleted.Select();
        if (sw == 2) mBtnShowAll.Select();
    }

    private IEnumerator Coroutine_WaitUntilMetadataLoaded()
    {
        while(!JigsawGameData.Instance.mMetaDataLoaded)
        {
            yield return null;
        }
        mBtnShowAll.onClick.Invoke();
        mBtnShowAll.Select();

        ImageMetaData data = JigsawGameData.Instance.GetCurrentImageData();
        SetImage(data);
    }

    void SetImage(ImageMetaData data)
    {
        if(data == null)
        {
            SetImageEmpty();
            return;
        }
        Texture2D tex = SpriteUtils.LoadTexture(data.filename);
        if(tex == null)
        {
            Debug.Log("Fatal error. Could not load image <" + data.filename + ">");
            return;
        }
        data.totalTiles = tex.width / 100 * tex.height / 100;
        mImage.sprite = SpriteUtils.CreateSpriteFromTexture2D(tex, 0, 0, tex.width, tex.height);
        mName.text = data.name;
        mCredit.text = "Photo by <b><color=yellow>" + data.credit + "</color></b> from Pexels";
        if (data.status == ImageMetaData.Status.COMPLETED)
        {
            mStatus.text = "<color=green> Completed </color> on " + data.completedDateTime.ToString("D", CultureInfo.CreateSpecificCulture("en-US"));

            System.TimeSpan t = System.TimeSpan.FromSeconds(data.secondsSinceStart);
            mTimeSpent.text = "You spent <color=yellow>" + 
                t.Hours.ToString() + " hr " + 
                t.Minutes.ToString() + " mins and " + 
                t.Seconds.ToString() + "sec</color>";
        }
        else if(data.status == ImageMetaData.Status.STARTED)
        {
            mStatus.text = "<color=yellow> Started </color>" + data.startDateTime.ToString("D", CultureInfo.CreateSpecificCulture("en-US"));

            System.TimeSpan t = System.TimeSpan.FromSeconds(data.secondsSinceStart);
            mTimeSpent.text = "You spent <color=yellow>" +
                t.Hours.ToString() + " hr " +
                t.Minutes.ToString() + " mins and " +
                t.Seconds.ToString() + "sec</color>";
        }
        else
        {
            mStatus.text = "<color=white> Not started </color>";// on 21 June, 2021
            data.tilesInPlace = 0;
            mTimeSpent.text = "";
        }
        mTiles.text = data.tilesInPlace.ToString() + "/<color=yellow>" + data.totalTiles.ToString() + "</color>";
    }

    void SetImageEmpty()
    {
        Texture2D tex = SpriteUtils.LoadTexture("empty");
        mImage.sprite = SpriteUtils.CreateSpriteFromTexture2D(tex, 0, 0, tex.width, tex.height);
        mName.text = "Empty";
        mCredit.text = "";
        mStatus.text = "";
        mTiles.text = "";
        mTimeSpent.text = "";
    }

    public void OnClickNextImage()
    {
        JigsawGameData.Instance.NextImage();
        SetImage(JigsawGameData.Instance.GetCurrentImageData());
    }

    public void OnClickPrevImage()
    {
        JigsawGameData.Instance.PreviousImage();
        SetImage(JigsawGameData.Instance.GetCurrentImageData());
    }

    public void OnClickPlay()
    {
        FadeSceneLoader.Instance.FadeSceneLoad("JigsawGame");
        //SceneManager.LoadScene("JigsawGame");
    }

    public void OnClickShowNowPlayingImagesList()
    {
        JigsawGameData.Instance.SetFilter(ImageMetaData.Status.STARTED);
        UpdateMenu();
        sw = 0;
        //Debug.Log("OnClickShowNowPlayingImagesList");
    }

    public void OnClickShowCompletedImagesList()
    {
        JigsawGameData.Instance.SetFilter(ImageMetaData.Status.COMPLETED);
        UpdateMenu();
        sw = 1;
        //Debug.Log("OnClickShowCompletedImagesList");
    }

    public void OnClickShowAllImagesList()
    {
        JigsawGameData.Instance.SetFilter(ImageMetaData.Status.NOT_STARTED);
        UpdateMenu();
        sw = 2;
        //Debug.Log("OnClickShowAllImagesList");
    }

    void UpdateMenu()
    {
        if (JigsawGameData.Instance.GetFilteredImageCount() == 0)
        {
            menu.BtnNext.gameObject.SetActive(false);
            menu.BtnHome.gameObject.SetActive(false);
            menu.BtnPlay.gameObject.SetActive(false);
        }
        else
        {
            menu.BtnNext.gameObject.SetActive(true);
            menu.BtnHome.gameObject.SetActive(true);
            menu.BtnPlay.gameObject.SetActive(true);
            //JigsawGameData.Instance.NextImage();
        }
        SetImage(JigsawGameData.Instance.GetCurrentImageData());
    }
}
