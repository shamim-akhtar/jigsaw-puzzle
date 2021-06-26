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

    private void Start()
    {

    }

    void Update()
    {
    }

}
