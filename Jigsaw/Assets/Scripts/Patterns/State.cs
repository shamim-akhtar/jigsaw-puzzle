using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Our objective is to implement a finite state machine using 
/// the class based approach.
/// 
/// What is a finite state mcahine?
/// A finite state machine is a computational pattern that models 
/// the state behaviour of a system. Such a system comprises a 
/// finite number of states and at any given point in time the
/// system exists in only one state.
/// 
/// What we need?
/// 1. State
/// This is a data structure (class) that encapsulates the 
/// state related functionlities.
/// 
/// 2. The State Machine itself. 
/// This is the class that will manage all the states and the transitions.
/// </summary>


namespace Patterns
{
    public class State<T>
    {
        // The name for the state.
        public string Name { get; set; }

        // The ID of the state.
        public T ID { get; private set; }

        public State(T id)
        {
            ID = id;
        }
        public State(T id, string name) : this(id)
        {
            Name = name;
        }

        public delegate void DelegateNoArg();

        public DelegateNoArg OnEnter;
        public DelegateNoArg OnExit;
        public DelegateNoArg OnUpdate;
        public DelegateNoArg OnFixedUpdate;

        public State(T id,
            DelegateNoArg onEnter,
            DelegateNoArg onExit = null,
            DelegateNoArg onUpdate = null,
            DelegateNoArg onFixedUpdate = null) : this(id)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnFixedUpdate = onFixedUpdate;
        }
        public State(T id, 
            string name,
            DelegateNoArg onEnter,
            DelegateNoArg onExit = null,
            DelegateNoArg onUpdate = null,
            DelegateNoArg onFixedUpdate = null) : this(id, name)
        {
            OnEnter = onEnter;
            OnExit = onExit;
            OnUpdate = onUpdate;
            OnFixedUpdate = onFixedUpdate;
        }

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

