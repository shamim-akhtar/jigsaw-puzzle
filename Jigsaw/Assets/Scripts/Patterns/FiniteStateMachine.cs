using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Patterns
{

    public class FiniteStateMachine<T>
    {
        /// <summary>
        /// We know that a state machine comprises many states (or finite number of states). 
        /// So how do we represent this? Probably a List? Or even better 
        /// a dictionary (why?)
        /// Because if we use a disctionary then we can associate each state with an enum /int/string that 
        /// represents the ID of the state.
        /// </summary>
        /// 

        protected Dictionary<T, State<T>> mStates; // this is the finite number of states that the state machine can have.
        protected State<T> mCurrentState;

        public FiniteStateMachine()
        {
            mStates = new Dictionary<T, State<T>>();
        }

        public void Add(State<T> state)
        {
            mStates.Add(state.ID, state);
        }

        public void Add(T stateID, State<T> state)
        {
            mStates.Add(stateID, state);
        }

        public State<T> GetState(T stateID)
        {
            return mStates[stateID];
        }

        public void SetCurrentState(T stateID)
        {
            State<T> state = mStates[stateID];
            SetCurrentState(state);
        }

        public State<T> GetCurrentState()
        {
            return mCurrentState;
        }

        public void SetCurrentState(State<T> state)
        {
            if (mCurrentState == state)
            {
                Debug.Log("Current state is same as previous state. No nothing changed!");
                return;
            }

            if (mCurrentState != null)
            {
                // we must do somethings? What to do?
                // inform the now current state that we are about to change?
                // or what else can we do?

                // one way of implementing this is to inform the state that you are exiting the state. // warn
                mCurrentState.Exit();
            }

            mCurrentState = state;

            // Once I changed the state, should the newly set current state do something?
            // like informing that a new state has been set?
            if (mCurrentState != null)
            {
                mCurrentState.Enter();
            }
        }

        public void Update()
        {
            if (mCurrentState != null)
            {
                mCurrentState.Update();
            }
        }

        public void FixedUpdate()
        {
            if (mCurrentState != null)
            {
                mCurrentState.FixedUpdate();
            }
        }
    }
}