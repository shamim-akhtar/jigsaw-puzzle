using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : Patterns.Singleton<GameApp>
{
  public bool TileMovementEnabled { get; set; } = false;
  public double SecondsSinceStart { get; set; } = 0;
  public int TotalTilesInCorrectPosition { get; set; } = 0;

  [SerializeField]
  List<string> jigsawImageNames = new List<string>();

  int imageIndex = 0;

  public string GetJigsawImageName()
  {
    string imageName = jigsawImageNames[imageIndex++];
    if(imageIndex == jigsawImageNames.Count)
    {
      imageIndex = 0;
    }
    return imageName;
  }
}
