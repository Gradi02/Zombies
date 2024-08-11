using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemyAI : StateMachine
{
    [SerializeField] private idleState _idleState;
    [SerializeField] private runState _runState;
    [SerializeField] private deathState _deathState;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //_runState.SetUpState(agent); 

        ChangeState(_idleState);
    }

    public void ChangeStateButton()
    {
        Debug.Log(currentState);
        if(currentState == _idleState)
            ChangeState(_runState);
        else
            ChangeState(_idleState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) ChangeStateButton();

        currentState?.DoUpdate();
        EnemyNavigation();
    }

    private void FixedUpdate()
    {
        currentState?.DoFixedUpdate();
    }

    private void EnemyNavigation()
    {
        if(currentState == _runState)
        {
            agent.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
        }
        else
        {
            agent.SetDestination(transform.position);
        }
    }

    public void DeathState()
    {
        ChangeState(_deathState, true);
    }
}
