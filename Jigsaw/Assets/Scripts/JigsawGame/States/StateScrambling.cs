using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns;

// The Loading Scrambling.
public class StateScrambling : State<JigsawGameStates>
{
    public JigsawGame Game { get; set; }
    public StateScrambling(JigsawGame game) : base()
    {
        Game = game;
        ID = JigsawGameStates.SCRAMBLING;
    }

    public override void Enter()
    {
        base.Enter();
        // We are shuffling. Disable Play btn and show the rest of bottom menu.
        Game.menu.SetActivePlayBtn(false);

        Game.Shuffle();
    }
}
