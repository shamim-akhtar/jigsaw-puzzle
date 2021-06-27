using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

// The Loading Playing.
public class StateCompleted : State<JigsawGameStates>
{
    public JigsawGame Game { get; set; }
    public StateCompleted(JigsawGame game) : 
        base(JigsawGameStates.COMPLETED)
    {
        Game = game;
    }

    public override void Enter()
    {
        base.Enter();
        Game.menu.TextWin.gameObject.SetActive(true);
        Game.menu.SetActivePlayBtn(true);
    }
}
