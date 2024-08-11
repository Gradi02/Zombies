using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class StateMachine : NetworkBehaviour
{
    [SerializeField] protected Animator animator;
    protected State currentState;
    private bool locked = false;
    
    protected NavMeshAgent agent;
    protected ZombieMainStates mainState = ZombieMainStates.chilling;

    private void Awake()
    {
        float rand = Random.Range(-0.10f, 0.10f);
        transform.localScale += new Vector3(rand,rand,rand);

        agent = GetComponent<NavMeshAgent>();
        State[] states = GetComponentsInChildren<State>();
        foreach (State s in states)
            s.Initialize(animator, Time.time, agent);
    }

    protected void ChangeState(State newState, bool _lock = false)
    {
        if((currentState == null || currentState != newState) && !locked)
        {
            currentState?.DoExit();
            Debug.Log("Change to: " + newState);

            currentState = newState;
            currentState.DoEnter();
        }

        if(_lock)
            locked = true;
    }
}

public enum ZombieMainStates
{
    chilling,
    alarmed,
    chasing,
    critical
}