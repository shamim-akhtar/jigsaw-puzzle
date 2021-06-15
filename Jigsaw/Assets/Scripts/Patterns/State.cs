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
        public State(int id,
            DelegateOnEnter onEnter,
            DelegateOnEnter onExit = null,
            DelegateOnEnter onUpdate = null,
            DelegateOnEnter onFixedUpdate = null) : base()
        {
            ID = id;
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnFixedUpdate = onFixedUpdate;
        }

        public delegate void DelegateOnEnter();
        public DelegateOnEnter OnEnter;
        public delegate void DelegateOnExit();
        public DelegateOnEnter OnExit;
        public delegate void DelegateOnUpdate();
        public DelegateOnEnter OnUpdate;
        public delegate void DelegateOnFixedUpdate();
        public DelegateOnEnter OnFixedUpdate;

        public string Name { get; set; }
        public int ID { get; set; }

        virtual public void Enter()
        {
            OnEnter?.Invoke();
        }

        virtual public void Exit()
        {
            OnExit?.Invoke();
        }
        virtual public void Update()
        {
            OnUpdate?.Invoke();
        }

        virtual public void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
    }

}

