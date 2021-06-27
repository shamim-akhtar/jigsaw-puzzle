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

        TileMovement.TileMovementEnabled = true;

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