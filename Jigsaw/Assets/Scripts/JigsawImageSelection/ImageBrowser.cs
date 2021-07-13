using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Globalization;

public class ImageBrowser : MonoBehaviour
{
    public Image mImage;
    public Text mName;
    public Text mCredit;
    public Text mStatus;
    public Text mTiles;
    public Text mTimeSpent;

    void Start()
    {
        StartCoroutine(Coroutine_WaitUntilMetadataLoaded());
    }

    void Update()
    {

    }

    private IEnumerator Coroutine_WaitUntilMetadataLoaded()
    {
        while(!JigsawGameData.Instance.mMetaDataLoaded)
        {
            yield return null;
        }
        SetImage(JigsawGameData.Instance.GetCurrentImageData());
    }

    void SetImage(ImageMetaData data)
    {
        Texture2D tex = SpriteUtils.LoadTexture(data.filename);
        if(tex == null)
        {
            Debug.Log("Fatal error. Could not load image <" + data.filename + ">");
            return;
        }
        data.totalTiles = tex.width / 100 * tex.height / 100;
        mImage.sprite = SpriteUtils.CreateSpriteFromTexture2D(tex, 0, 0, tex.width, tex.height);
        mName.text = data.name;
        mCredit.text = data.credit;
        if(data.status == ImageMetaData.Status.COMPLETED)
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
        SceneManager.LoadScene("JigsawGame");
    }
}
