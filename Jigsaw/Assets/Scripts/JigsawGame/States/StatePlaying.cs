using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;


// The Loading Playing.
public class StatePlaying : State<JigsawGameStates>
{
    public JigsawGame Game { get; set; }
    public StatePlaying(JigsawGame game) : 
        base(JigsawGameStates.PLAYING)
    {
        Game = game;
    }

    public override void Enter()
    {
        base.Enter();

        // Enabble back camera movement and menu input handling.
        CameraMovement.CameraPanning = true;
        Menu.Enabled = true;

        TileMovement.TileMovementEnabled = true;
        Game.menu.SetActivePlayBtn(false);

        JigsawGameData.Instance.SetCurrentImageDataStatus(JigsawGameData.Status.STARTED);
        for (int i = 0; i < Game.NumTilesX; i++)
        {
            for (int j = 0; j < Game.NumTilesY; ++j)
            {
                TileMovement tile = Game.mTileGameObjects[i, j].GetComponent<TileMovement>();
                tile.ApplyTileInPlace();
            }
        }

        // Start the timer.
        Game.StartTimer();
    }

    public override void Exit()
    {
        base.Exit();

        // stop the timer.
        Game.StopTimer();
    }
}