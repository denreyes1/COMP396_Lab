using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{

    public class State
    {
        public string name;
        public System.Action OnFrame;
        public System.Action OnEnter;
        public System.Action OnExit;

        public override string ToString()
        {
            return name;
        }
    }

    public bool ready = false;
    public Dictionary<string, State> states = new Dictionary<string, State>();

    public State currentState { get; private set; }

    [SerializeField] State initialState;

    public State CreateState(string name)
    {
        State state = new State();
        state.name = name;
        if (states.Count == 0)
        {
            initialState = state;
        }
        states[name] = state;
        return state;
    }
    
    public void Update()
    {
        if (!ready)
            return;
        
        if (states.Count == 0 || initialState == null)
        {
            Debug.LogError("State Machine with no states!?!");
            return;
        }

        if (currentState == null)
        {
            TransitionTo(initialState);
        }
        
        if(currentState.OnFrame != null)
        {
            currentState.OnFrame();
        }
    }

    public void TransitionTo(State newState)
    {
        if (newState == null)
        {
            Debug.LogError("Trying to Transition to a null state!?!");
            return;
        }
        //currentState?.OnExit?.Invoke();
        if(currentState != null && currentState.OnExit != null)
        {
            currentState.OnExit();
        }
        Debug.Log($"Transitioning from state {currentState} to {newState}.");
        currentState = newState;
        //currentState?.OnEnter?.Invoke();
        //if (currentState != null && currentState.OnEnter != null)
        //{
        //    currentState.OnEnter();
        //}

    }

    public void TransitionTo(string stateName)
    {
        if(states.ContainsKey(stateName))
        {
            Debug.LogError($"State machine doesn't contain state {stateName}!?!");
            return;
        }
        TransitionTo(states[stateName]);
    }
}
