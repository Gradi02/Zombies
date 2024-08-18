using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chillState : State
{
    [SerializeField] private idleState _idleState;
    [SerializeField] private walkState _walkState;

    private float minIdleTime = 3f, maxIdleTime = 6f;
    private float minWalkTime = 5f, maxWalkTime = 15f;
    private float timeToChangeState = 0f;

    public override void DoEnter()
    {
        base.DoEnter();

        agent.enabled = true;
        timeToChangeState = time + Random.Range(minIdleTime, maxIdleTime);
        machine.ChangeSubState(_idleState);
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        SelectSubState();       
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();

        isCompleted = false;

        if (agent.enabled)
        {
            agent.ResetPath();
            agent.enabled = false;
        }
    }


    private void SelectSubState()
    {
        if (time > timeToChangeState)
        {
            if (subState == _idleState)
            {
                machine.ChangeSubState(_walkState);

                agent.enabled = true;
                agent.destination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));

                timeToChangeState = time + Random.Range(minWalkTime, maxWalkTime);
            }
            else if (subState == _walkState)
            {
                machine.ChangeSubState(_idleState);

                agent.ResetPath();
                agent.enabled = false;

                timeToChangeState = time + Random.Range(minIdleTime, maxIdleTime);
            }
        }

        if (agent.enabled && agent.remainingDistance < agent.radius)
        {
            agent.destination = transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
        }
    }
}
