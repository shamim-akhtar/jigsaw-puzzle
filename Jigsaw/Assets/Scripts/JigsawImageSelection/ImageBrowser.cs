using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageBrowser : MonoBehaviour
{
    public Image mImage;
    public Text mName;
    public Text mCredit;
    public Text mStatus;
    public Text mTiles;

    void Start()
    {
        SetImage(JigsawGameData.Instance.GetCurrentImageData());
    }

    void Update()
    {
        
    }

    void SetImage(JigsawGameData.ImageData data)
    {
        Texture2D tex = SpriteUtils.LoadTexture(data.filename);
        data.totalTiles = tex.width / 100 * tex.height / 100;
        mImage.sprite = SpriteUtils.CreateSpriteFromTexture2D(tex, 0, 0, tex.width, tex.height);
        mName.text = data.name;
        mCredit.text = data.credit;
        if(data.status == JigsawGameData.Status.COMPLETED)
        {
            mStatus.text = "<color=green> Completed </color>";// on 21 June, 2021
        }
        else if(data.status == JigsawGameData.Status.STARTED)
        {
            mStatus.text = "<color=yellow> Started </color>";// on 21 June, 2021
        }
        else
        {
            mStatus.text = "<color=white> Not started </color>";// on 21 June, 2021
            data.tilesInPlace = 0;
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
