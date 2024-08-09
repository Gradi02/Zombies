using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StateMachine : NetworkBehaviour
{
    [SerializeField] protected Animator animator;
    protected State currentState;

    private void Awake()
    {
        State[] states = GetComponentsInChildren<State>();
        foreach (State s in states)
            s.Initialize(animator, Time.time);
    }

    protected void ChangeState(State newState)
    {
        if(currentState == null || currentState != newState)
        {
            currentState?.DoExit();
            Debug.Log("Change to: " + newState);

            currentState = newState;
            currentState.DoEnter();
        }
    }
}
