using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

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
     
    public struct ImageData
    {
        public string filename;
        public string name;
        public string credit;
        public Status status;
        public Layout layout;
        public int tilesInPlace;
        public int totalTiles;
    }

    public List<ImageData> mImageDataList = new List<ImageData>();

    public double mSecondsSinceStart = 0;
    public int mTotalTilesInCorrectPosition = 0;
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

    private void LoadImageDataList()
    {
        mImageDataList.Add(new ImageData
        {
            name = "Red Trees",
            credit = "Photo by <b><color=yellow>Artem Saranin</color></b> from Pexels",
            filename = "Images/Jigsaw/8x12/pexels-alleksana-4238349",
            layout = Layout.PORTRAIT,
            status = Status.NOT_STARTED
        });

        mImageDataList.Add(new ImageData
        {
            name = "Closeup Photo of Slice of Orange",
            credit = "Photo by <b><color=yellow>Engin Akyurt</color></b> from Pexels",
            filename = "Images/Jigsaw/8x12/pexels-engin-akyurt-1435735",
            layout = Layout.PORTRAIT,
            status = Status.NOT_STARTED
        });

        mImageDataList.Add(new ImageData
        {
            name = "Red Tower Hill Bus",
            credit = "Photo by <b><color=yellow>Oleg Magni</color></b> from Pexels",
            filename = "Images/Jigsaw/8x12/pexels-oleg-magni-1837590",
            layout = Layout.PORTRAIT,
            status = Status.NOT_STARTED
        });
    }
}
