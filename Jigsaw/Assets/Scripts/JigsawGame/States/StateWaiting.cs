using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;


// The Loading state.
public class StateWaiting : State<JigsawGameStates>
{
    public JigsawGame Game { get; set; }
    public StateWaiting(JigsawGame game) 
        : base(JigsawGameStates.WAITING)
    {
        Game = game;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}