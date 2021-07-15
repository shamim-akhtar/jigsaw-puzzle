using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;
using System.IO;
using System.Runtime.Serialization;

public class JigsawGameData : Singleton<JigsawGameData>
{
    ImageMetaData[] mImageDataList;
    public bool mMetaDataLoaded = false;

    private int mIndex = 0;

    List<ImageMetaData> mFilteredImages = new List<ImageMetaData>();


    public void NextImage()
    {
        mIndex++;
        if (mIndex >= mFilteredImages.Count)
        {
            mIndex = 0;
        }
    }

    public void PreviousImage()
    {
        --mIndex;
        if(mIndex < 0)
        {
            mIndex = mFilteredImages.Count- 1;
        }
    }

    public string GetImageFilename()
    {
        return mFilteredImages[mIndex].filename;
    }

    public ImageMetaData GetCurrentImageData()
    {
        if(mFilteredImages.Count > 0)
        {
            return mFilteredImages[mIndex];
        }
        return null;
    }

    public void SetCurrentImageDataStatus(ImageMetaData.Status status)
    {
        ImageMetaData data = mFilteredImages[mIndex];
        data.status = status;
    }

    public int GetFilteredImageCount()
    {
        return mFilteredImages.Count;
    }

    private void Start()
    {
        LoadMetaData();
        mMetaDataLoaded = true;
    }

    IEnumerator Coroutine_AutoSave()
    {
        while(true)
        {
            yield return new WaitForSeconds(15.0f);
            SaveMetaData();
        }
    }

    private bool LoadMetaData()
    {
        string filename = Application.persistentDataPath + "/metadata.json";
        if (!File.Exists(filename))
        {
            filename = "metadata";
            Debug.Log(filename);
            TextAsset targetFile = Resources.Load<TextAsset>(filename);
            mImageDataList = JsonHelper.FromJson<ImageMetaData>(targetFile.text);
        }
        else
        {
            StreamReader inStream = File.OpenText(filename);
            string jsonString = inStream.ReadToEnd();
            inStream.Close();
            mImageDataList = JsonHelper.FromJson<ImageMetaData>(jsonString);

            if(mImageDataList.Length == 0)
            {
                TextAsset targetFile = Resources.Load<TextAsset>("metadata");
                mImageDataList = JsonHelper.FromJson<ImageMetaData>(targetFile.text);
            }
        }

        return true;
    }

    public void SaveMetaData()
    {
        string filename = Application.persistentDataPath + "/metadata.json";

        string jsonString = JsonHelper.ToJson(mImageDataList, true);

        StreamWriter outStream = File.CreateText(filename);
        outStream.Write(jsonString);
        outStream.Close();
    }

    public void SetFilter(ImageMetaData.Status status)
    {
        mFilteredImages.Clear();
        mIndex = 0;
        switch (status)
        {
            case ImageMetaData.Status.COMPLETED:
                {
                    for(int i = 0; i <mImageDataList.Length; ++i)
                    {
                        if(mImageDataList[i].status == ImageMetaData.Status.COMPLETED)
                        {
                            mFilteredImages.Add(mImageDataList[i]);
                        }
                    }
                    break;
                }
            case ImageMetaData.Status.STARTED:
                {
                    for (int i = 0; i < mImageDataList.Length; ++i)
                    {
                        if (mImageDataList[i].status == ImageMetaData.Status.STARTED)
                        {
                            mFilteredImages.Add(mImageDataList[i]);
                        }
                    }
                    break;
                }
            default:
                {
                    mFilteredImages.AddRange(mImageDataList);
                    break;
                }
        }
    }

    void OnApplicationPause(bool flag)
    {
        if (flag)
        {
            SaveMetaData();
        }
    }
}
