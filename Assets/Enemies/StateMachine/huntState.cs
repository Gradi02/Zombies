using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class huntState : State
{
    [SerializeField] private runState _runState;
    [SerializeField] private idleState _idleState;

    private float minInterrestTime = 15f, maxInterrestTime = 25f;
    private float timeToComplete, timeToNextPoint = 0;
    private float searchDistance = 20f;

    public override void DoEnter()
    {
        base.DoEnter();
        timeToComplete = time + Random.Range(minInterrestTime, maxInterrestTime);

        agent.enabled = true;
        machine.ChangeSubState(_runState);
        agent.destination = targetPos;
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        if (time > timeToComplete)
            isCompleted = true;

        if (subState == _runState)
        {
            if (agent.enabled && agent.remainingDistance < agent.radius)
            {
                timeToNextPoint = time + Random.Range(1f, 2f);
                machine.ChangeSubState(_idleState);
            }
        }
        else if(time > timeToNextPoint)
        {
            machine.ChangeSubState(_runState);
            agent.destination = GetNewPlaceToCheck();
        }
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
        
        isCompleted = false;
        agent.ResetPath();
    }

    private Vector3 GetNewPlaceToCheck()
    {
        return targetPos + new Vector3(Random.Range(-searchDistance, searchDistance), 0, Random.Range(-searchDistance, searchDistance));
    }
}
