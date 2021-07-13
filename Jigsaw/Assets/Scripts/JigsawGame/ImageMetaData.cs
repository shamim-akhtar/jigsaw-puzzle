using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ImageMetaData
{
    [System.Serializable]
    public enum Status
    {
        NOT_STARTED,
        STARTED,
        COMPLETED,
    }

    [System.Serializable]
    public enum Layout
    {
        LANDSCAPE,
        PORTRAIT,
    }

    public string filename;
    public string name;
    public string credit;
    public Status status = Status.NOT_STARTED;
    public Layout layout;
    public int tilesInPlace = 0;
    public int totalTiles;
    public double secondsSinceStart = 0;
    public System.DateTime startDateTime;
    public System.DateTime completedDateTime;
}
