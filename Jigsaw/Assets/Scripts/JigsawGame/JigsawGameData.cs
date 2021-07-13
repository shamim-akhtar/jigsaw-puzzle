using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;
using System.IO;
using System.Runtime.Serialization;

public class JigsawGameData : Singleton<JigsawGameData>
{
    //public List<ImageMetaData> mImageDataList = new List<ImageMetaData>();
    ImageMetaData[] mImageDataList;
    public bool mMetaDataLoaded = false;

    private int mIndex = 0;

    public void NextImage()
    {
        mIndex++;
        if (mIndex >= mImageDataList.Length)
        {
            mIndex = 0;
        }
    }

    public void PreviousImage()
    {
        --mIndex;
        if(mIndex < 0)
        {
            mIndex = mImageDataList.Length - 1;
        }
    }

    public string GetImageFilename()
    {
        return mImageDataList[mIndex].filename;
    }

    public ImageMetaData GetCurrentImageData()
    {
        return mImageDataList[mIndex];
    }

    public void SetCurrentImageDataStatus(ImageMetaData.Status status)
    {
        ImageMetaData data = mImageDataList[mIndex];
        data.status = status;
    }

    //private void LoadImageDataList()
    //{
    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Red Trees",
    //    //    credit = "Photo by <b><color=yellow>Artem Saranin</color></b> from Pexels",
    //    //    filename = "pexels-alleksana-4238349",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Closeup Photo of Slice of Orange",
    //    //    credit = "Photo by <b><color=yellow>Engin Akyurt</color></b> from Pexels",
    //    //    filename = "pexels-engin-akyurt-1435735",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Red Tower Hill Bus",
    //    //    credit = "Photo by <b><color=yellow>Oleg Magni</color></b> from Pexels",
    //    //    filename = "pexels-oleg-magni-1837590",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Brown Wooden Dock on Body of Water",
    //    //    credit = "Photo by <b><color=yellow>Maxim Kovalev</color></b> from Pexels",
    //    //    filename = "pexels-maxim-kovalev-8647586",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Rain Passing Through at Clearing in the Woods",
    //    //    credit = "Photo by <b><color=yellow>Darius Krause</color></b> from Pexels",
    //    //    filename = "pexels-darius-krause-2609106",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Rain Passing Through at Clearing in the Woods",
    //    //    credit = "Photo by <b><color=yellow>Maria Orlova</color></b> from Pexels",
    //    //    filename = "pexels-maria-orlova-4915526",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Woman Behind Blue Leaves",
    //    //    credit = "Photo by <b><color=yellow>radiographie</color></b> from Pexels",
    //    //    filename = "pexels-radiographie-2728784",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Woman Hiding Behind Wine Glass",
    //    //    credit = "Photo by <b><color=yellow>Julia Kuzenkov</color></b> from Pexels",
    //    //    filename = "pexels-julia-kuzenkov-2817087",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Picturesque sunset over sea with palms",
    //    //    credit = "Photo by <b><color=yellow>Eman Genatilan</color></b> from Pexels",
    //    //    filename = "pexels-eman-genatilan-5170362",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Narrow staircase with potted flowers",
    //    //    credit = "Photo by <b><color=yellow>Ryutaro Tsukata</color></b> from Pexels",
    //    //    filename = "pexels-ryutaro-tsukata-5472528",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Brown Concrete Buildings With Stairs",
    //    //    credit = "Photo by <b><color=yellow>Kristina Paukshtite</color></b> from Pexels",
    //    //    filename = "pexels-kristina-paukshtite-1337713",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Lighted Up Alleyway",
    //    //    credit = "Photo by <b><color=yellow>Sunyu Kim</color></b> from Pexels",
    //    //    filename = "pexels-sunyu-kim-1294671",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Colorful Painted Buildings",
    //    //    credit = "Photo by <b><color=yellow>Raul Juarez</color></b> from Pexels",
    //    //    filename = "pexels-raul-juarez-2388639",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Empty Lighted City Street at Night",
    //    //    credit = "Photo by <b><color=yellow>Ethan Brooke</color></b> from Pexels",
    //    //    filename = "pexels-ethan-brooke-2128040",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Shelves and container with magazines",
    //    //    credit = "Photo by <b><color=yellow>Ekrulila</color></b> from Pexels",
    //    //    filename = "pexels-ekrulila-7345377",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Cobblestone street",
    //    //    credit = "Photo by <b><color=yellow>Maria Orlova</color></b> from Pexels",
    //    //    filename = "pexels-maria-orlova-4916146",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Red Vehicle on Road Beside Building",
    //    //    credit = "Photo by <b><color=yellow>Aleksandar Pasaric</color></b> from Pexels",
    //    //    filename = "pexels-aleksandar-pasaric-2481670",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Grey Concrete Road at Night",
    //    //    credit = "Photo by <b><color=yellow>Artem Lysenko</color></b> from Pexels",
    //    //    filename = "pexels-artem-lysenko-2301177",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Breathtaking seascape with amazing coastal village on cliff",
    //    //    credit = "Photo by <b><color=yellow>Samir BELHAMRA</color></b> from Pexels",
    //    //    filename = "pexels-grafixartphoto-samir-belhamra-4254555",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Staircase in blue city of Chefchaouen",
    //    //    credit = "Photo by <b><color=yellow>Ryutaro Tsukata</color></b> from Pexels",
    //    //    filename = "pexels-ryutaro-tsukata-5472522",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Multicolored Umbrella",
    //    //    credit = "Photo by <b><color=yellow>Ekrulila</color></b> from Pexels",
    //    //    filename = "pexels-ekrulila-2218344",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Green Grass Field Near Mountain",
    //    //    credit = "Photo by <b><color=yellow>Gianluca Grisenti</color></b> from Pexels",
    //    //    filename = "pexels_greengrassfield",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Sakura Tree",
    //    //    credit = "Photo by <b><color=yellow>Oleg Magni</color></b> from Pexels",
    //    //    filename = "pexels-oleg-magni-2033997",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Pink Red Yellow Petaled Flower",
    //    //    credit = "Photo by <b><color=yellow>Unchalee Srirugsar</color></b> from Pexels",
    //    //    filename = "pexels-unchalee-srirugsar-85773",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Tiger in Shallow Photo",
    //    //    credit = "Photo by <b><color=yellow>Richard Verbeek</color></b> from Pexels",
    //    //    filename = "pexels-richard-verbeek-572861",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Aerial View Photography of Villages",
    //    //    credit = "Photo by <b><color=yellow>Tobias Bjørkli</color></b> from Pexels",
    //    //    filename = "pexels-tobias-bjørkli-1559821",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});

    //    //mImageDataList.Add(new ImageData
    //    //{
    //    //    name = "Aerial View Photography of Villages",
    //    //    credit = "Photo by <b><color=yellow>Tobias Bjørkli</color></b> from Pexels",
    //    //    filename = "pexels-tobias-bjørkli-1559821",
    //    //    layout = Layout.PORTRAIT,
    //    //    status = Status.NOT_STARTED
    //    //});
    //}

    private void Start()
    {
        //LoadImageDataList();
        LoadMetaData();
        mMetaDataLoaded = true;
    }

    private bool LoadMetaData()
    {
        string filename = Application.persistentDataPath + "/metadata.json";
        if (!File.Exists(filename))
        {
            filename = Application.dataPath + "/Resources/metadata.json";
        }
        Debug.Log(filename);
        StreamReader inStream = System.IO.File.OpenText(filename);
        string jsonString = inStream.ReadToEnd();
        inStream.Close();

        mImageDataList = JsonHelper.FromJson<ImageMetaData>(jsonString);
        return true;
    }

    public void SaveMetaData()
    {
        string filename = Application.persistentDataPath + "/metadata.json";

        string jsonString = JsonHelper.ToJson(mImageDataList, true);

        StreamWriter outStream = System.IO.File.CreateText(filename);
        outStream.Write(jsonString);
        outStream.Close();
    }
}
