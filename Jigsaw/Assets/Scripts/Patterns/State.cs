using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Our objective is to implement a finite state machine using the class based approach.
/// What is a finite state mcahine?
/// A finite state machine is a computational pattern that models the state behaviour of a system.
/// Such a system comprises a finite number of states and at any given point in time the
/// system exists in only one state.
/// 
/// What we need?
/// 1. State
/// This is a data structure (class) that encapsulates the state related functionlities.
/// 
/// 2. The State Machine itself. 
/// This is the class that will manage all the states and the transitions.
/// </summary>
/// 


namespace Patterns
{
    // we create the state class.
    public class State
    {
        public State()
        {

        }

        public string Name { get; set; }
        public int ID { get; set; }

        virtual public void Enter()
        {
            //Debug.Log("State: " + Name);
        }
        virtual public void Exit()
        {
            //Debug.Log(Name + " - State exiting.");
        }
        virtual public void Update()
        {

        }

        virtual public void FixedUpdate()
        {

        }
    }

}

