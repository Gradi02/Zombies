using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyAI : StateMachine
{
    [SerializeField] private idleState _idleState;
    [SerializeField] private runState _runState;
    [SerializeField] private deathState _deathState;
    [SerializeField] private walkState _walkState;


    private void Start()
    {
        ChangeState(_walkState);

        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    private void Update()
    {
        currentState?.DoUpdate();
        AIController();
    }

    private void FixedUpdate()
    {
        currentState?.DoFixedUpdate();
    }

    private void AIController()
    {
        switch(mainState)
        {
            case ZombieMainStates.chilling:
                {
                    ChillingStateController();
                    break;
                }
            case ZombieMainStates.alarmed:
                {

                    break;
                }
            case ZombieMainStates.chasing:
                {

                    break;
                }
            case ZombieMainStates.critical:
                {

                    break;
                }
        }
    }

    [Header("Chilling State Variables")]
    private float walkingSpeed = 1;
    private float minIdleTime = 3f, maxIdleTime = 6f;
    private float minWalkTime = 5f, maxWalkTime = 10f;
    private float timeToChangeState = 0f;

    private void ChillingStateController()
    {
        if (Time.time > timeToChangeState)
        {
            if (currentState == _idleState)
            {
                agent.destination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                ChangeState(_walkState);
                agent.updateRotation = true;
                timeToChangeState = Time.time + Random.Range(minWalkTime, maxWalkTime);
            }
            else if (currentState == _walkState)
            {
                agent.ResetPath();
                ChangeState(_idleState);
                agent.updateRotation = false;
                timeToChangeState = Time.time + Random.Range(minIdleTime, maxIdleTime);
            }
        }

        if(agent.remainingDistance < agent.radius)
        {
            agent.destination = transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
        }
    }

    void OnAnimatorMove()
    {
        Vector3 position = animator.rootPosition;
        transform.position = position;
        agent.nextPosition = transform.position;
    }

    public void DeathState()
    {
        ChangeState(_deathState, true);
    }
}
