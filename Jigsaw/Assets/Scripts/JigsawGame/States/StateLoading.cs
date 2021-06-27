using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;


// The Loading state.
public class StateLoading : State<JigsawGameStates>
{
    public JigsawGame Game { get; set; }
    public StateLoading(JigsawGame game) 
        : base(JigsawGameStates.LOADING)
    {
        Game = game;
    }

    public override void Enter()
    {
        base.Enter();

        // Disable camera movement and menu input handling.
        CameraMovement.CameraPanning = false;
        Menu.Enabled = false;
        TileMovement.TileMovementEnabled = false;

        Game.menu.SetActivePlayBtn(true);

        Game.LoadLevel();
    }

    public override void Update()
    {
        base.Update();
        if(Game.LoadingFinished)
        {
            Game.OnFinishedLoading();
        }
    }

    public override void Exit()
    {
        base.Exit();

        //// Enabble back camera movement and menu input handling.
        //CameraMovement.CameraPanning = true;
        //Menu.Enabled = true;

        //Game.menu.SetActivePlayBtn(false);
    }
}