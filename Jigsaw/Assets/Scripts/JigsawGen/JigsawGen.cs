using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// A script that helps with the creation of the 
// Jigsaw puzzle and the associated data.
public class JigsawGen : MonoBehaviour
{
    //public List<Sprite> Images = new List<Sprite>();

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

    [System.Serializable]
    public class ImageMetaData
    {
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

    void RenameInagesAndCreateJSon()
    {
        string path = Application.dataPath + "/Resources";

        string[] files = System.IO.Directory.GetFiles(path, "*.jpg");

        string destinationPath = path + "/jig";
        System.IO.Directory.CreateDirectory(destinationPath);

        List<ImageMetaData> images = new List<ImageMetaData>();

        for (int i = 0; i < files.Length; ++i)
        {
            //Debug.Log(files[i]);
            string destinationFilename = destinationPath + "/image_" + i.ToString("D3") + ".jpg";
            System.IO.File.Copy(files[i], destinationFilename, true);

            ImageMetaData meta = new ImageMetaData();
            meta.filename = destinationFilename.Substring(path.Length);

            string inputFilename = System.IO.Path.GetFileNameWithoutExtension(files[i]);
            string[] subs = inputFilename.Split('-');
            if (subs.Length > 2)
            {
                string credit = "";
                for (int j = 1; j < subs.Length - 1; j++)
                {
                    credit += char.ToUpper(subs[j][0]) + subs[j].Substring(1) + " ";
                }

                meta.credit = credit;
                meta.name = subs[subs.Length - 1];
            }
            images.Add(meta);
        }

        //Convert to JSON
        string jsonString = JsonHelper.ToJson(images.ToArray(), true);
        string metaFilename = path + "/metadata.json";

        StreamWriter outStream = System.IO.File.CreateText(metaFilename);
        outStream.WriteLine(jsonString);
        outStream.Close();
    }

    // Start is called before the first frame update
    void Start()
    {
        RenameInagesAndCreateJSon();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
