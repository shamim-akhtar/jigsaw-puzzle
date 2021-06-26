using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

public class JigsawGameData : Singleton<JigsawGameData>
{
    // The image to be loaded for the level.
    public string mFilename;
    public double mSecondsSinceStart = 0;
    public int mTotalTilesInCorrectPosition = 0;
    //public int mImageIndex = 0;

    public List<(string, string)> mNamedImages = new List<(string, string)>()
    {
        ("Nature",          "images/image01_8_5"),
        ("Beautiful Town",  "images/picture01_15_10"),
        ("Amira",           "images/Jigsaw/amira_10_7"),
        ("Flowers",         "images/Jigsaw/flower01_12_8"),
        ("Nature 2",        "images/Jigsaw/nature01_20_13")
    };

    private void Start()
    {

    }

    void Update()
    {
    }

}
