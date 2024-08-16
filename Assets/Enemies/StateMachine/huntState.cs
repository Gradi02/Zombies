using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class huntState : State
{
    [SerializeField] private runState _runState;
    [SerializeField] private idleState _idleState;

    private float minInterrestTime = 10f, maxInterrestTime = 20f;
    private float timeToComplete, timeToNextPoint = 0;
    private float searchDistance = 10f;

    public override void DoEnter()
    {
        base.DoEnter();
        timeToComplete = time + Random.Range(minInterrestTime, maxInterrestTime);

        agent.enabled = true;
        machine.ChangeSubState(_runState);
        agent.destination = GetNewPlaceToCheck();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        if (time > timeToComplete)
            isCompleted = true;

        if (subState == _runState)
        {
            Vector3 dir = transform.position - agent.destination;
            if (dir.sqrMagnitude < 5)
            {
                timeToNextPoint = time + Random.Range(2f, 3f);
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
