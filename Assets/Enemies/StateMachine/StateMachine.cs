using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class StateMachine : NetworkBehaviour
{
    [SerializeField] protected Animator animator;
    protected State currentState;
    public State subState { get; private set; }
    private bool locked = false;
    
    protected NavMeshAgent agent;
    private Rigidbody body;

    protected State[] states;

    private void Awake()
    {
        float rand = Random.Range(-0.10f, 0.10f);
        transform.localScale += new Vector3(rand,rand,rand);

        body = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        states = GetComponentsInChildren<State>();
        foreach (State s in states)
            s.Initialize(animator, Time.time, agent, body, this);
    }

    protected void ChangeState(State newState, bool _lock = false, bool forceReenter = false)
    {
        if (((currentState == null || currentState != newState) && !locked) || forceReenter)
        {
            currentState?.DoExit();
            Debug.Log("Change to: " + newState);

            currentState = newState;
            currentState.DoEnter();
        }

        if(_lock)
            locked = true;
    }

    public void ChangeSubState(State newSubState, bool force = false)
    {
        if (subState == null || subState != newSubState || force)
        {
            subState?.DoExit();
            //Debug.Log("Change Substate to: " + newSubState);

            subState = newSubState;
            subState.DoEnter();
        }
    }
}