using System;
using System.Collections.Generic;
using System.Text;

namespace Patterns
{
    public class FiniteStateMachine<T>
    {
        // A Finite State Machine
        //    - consists of a set of states,
        //    - and at any given time, an FSM can exist in only one 
        //      State out of these possible set of states.

        // A dictionary to represent the a set of states.
        protected Dictionary<T, State<T>> mStates;

        // The current state.
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
            if(mStates.ContainsKey(stateID))
                return mStates[stateID];
            return null;
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
                return;
            }

            if (mCurrentState != null)
            {
                mCurrentState.Exit();
            }

            mCurrentState = state;

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
