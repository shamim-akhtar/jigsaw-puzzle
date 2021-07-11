using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;
using System.IO;
using System.Runtime.Serialization;

public class JigsawGameData : Singleton<JigsawGameData>
{
    public enum Status
    {
        NOT_STARTED,
        STARTED,
        COMPLETED,
    }

    public enum Layout
    {
        LANDSCAPE,
        PORTRAIT,
    }
     
    public class ImageData
    {
        public string filename;
        public string name;
        public string credit;
        public Status status = Status.NOT_STARTED;
        public Layout layout;
        public int tilesInPlace = 0;
        public int totalTiles;
        public double secondsSinceStart = 0;
    }

    public List<ImageData> mImageDataList = new List<ImageData>();
    public bool mMetaDataLoaded = false;

    //public double mSecondsSinceStart = 0;
    //public int mTotalTilesInCorrectPosition = 0;
    private int mIndex = 0;

    public void NextImage()
    {
        mIndex++;
        if (mIndex >= mImageDataList.Count)
        {
            mIndex = 0;
        }
    }

    public void PreviousImage()
    {
        --mIndex;
        if(mIndex < 0)
        {
            mIndex = mImageDataList.Count - 1;
        }
    }

    public string GetImageFilename()
    {
        return mImageDataList[mIndex].filename;
    }

    public ImageData GetCurrentImageData()
    {
        return mImageDataList[mIndex];
    }

    JigsawGameData() : base()
    {
        LoadImageDataList();
    }

    public void SetCurrentImageDataStatus(JigsawGameData.Status status)
    {
        ImageData data = mImageDataList[mIndex];
        data.status = status;
    }

    private void LoadImageDataList()
    {
        mImageDataList.Add(new ImageData
        {
            name = "Red Trees",
            credit = "Photo by <b><color=yellow>Artem Saranin</color></b> from Pexels",
            filename = "pexels-alleksana-4238349",
            layout = Layout.PORTRAIT,
            status = Status.NOT_STARTED
        });

        mImageDataList.Add(new ImageData
        {
            name = "Closeup Photo of Slice of Orange",
            credit = "Photo by <b><color=yellow>Engin Akyurt</color></b> from Pexels",
            filename = "pexels-engin-akyurt-1435735",
            layout = Layout.PORTRAIT,
            status = Status.NOT_STARTED
        });

        mImageDataList.Add(new ImageData
        {
            name = "Red Tower Hill Bus",
            credit = "Photo by <b><color=yellow>Oleg Magni</color></b> from Pexels",
            filename = "pexels-oleg-magni-1837590",
            layout = Layout.PORTRAIT,
            status = Status.NOT_STARTED
        });

    }

    private void Start()
    {
        LoadMetaData();
        mMetaDataLoaded = true;
    }

    private bool LoadMetaData()
    {
        string filename = Application.persistentDataPath + "/metadata";
        if (File.Exists(filename))
        {
            using (BinaryReader Reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                for (int i = 0; i < mImageDataList.Count; ++i)
                {
                    ImageData data = mImageDataList[i];
                    data.status = (JigsawGameData.Status)Reader.ReadInt32();
                    data.tilesInPlace = Reader.ReadInt32();
                    data.secondsSinceStart = Reader.ReadDouble();
                }
            }
            return true;
        }
        return false;
    }

    public void SaveMetaData()
    {
        BinaryWriter Writer = null;
        string filename = Application.persistentDataPath + "/metadata";

        try
        {
            Writer = new BinaryWriter(File.OpenWrite(filename));
            for (int i = 0; i < mImageDataList.Count; ++i)
            {
                ImageData data = mImageDataList[i];
                Writer.Write((int)data.status);
                Writer.Write(data.tilesInPlace);
                Writer.Write(data.secondsSinceStart);
            }

            Writer.Close();
        }
        catch (SerializationException e)
        {
            Debug.Log("Failed to save image metadata. Reason: " + e.Message);
            throw;
        }
    }
}
