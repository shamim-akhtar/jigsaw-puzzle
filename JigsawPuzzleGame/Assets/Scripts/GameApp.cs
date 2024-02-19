using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameApp : Patterns.Singleton<GameApp>
{
  public bool TileMovementEnabled { get; set; } = false;
  public double SecondsSinceStart { get; set; } = 0;
  public int TotalTilesInCorrectPosition { get; set; } = 0;
}
