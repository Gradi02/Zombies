using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class huntState : State
{
    [SerializeField] private runState _runState;
    [SerializeField] private idleState _idleState;

    private float minInterrestTime = 15f, maxInterrestTime = 25f;
    private float timeToComplete, timeToNextPoint = 0;
    private float searchDistance = 20f;
    [SerializeField] private LayerMask mask;

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
            agent.path = GetNewPlaceToCheck();
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
        agent.enabled = false;
    }

    private NavMeshPath GetNewPlaceToCheck()
    {
        bool pathCorrect = true;
        NavMeshPath navMeshPath = new NavMeshPath();
        while (pathCorrect)
        {
            Vector3 newDestRay = transform.position + new Vector3(Random.Range(-searchDistance, searchDistance), 1000, Random.Range(-searchDistance, searchDistance));
            if (Physics.Raycast(newDestRay, Vector3.down, out RaycastHit hit, 1100, mask))
            {
                if (agent.CalculatePath(agent.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    pathCorrect = false;
                }
                else
                {
                    navMeshPath = new NavMeshPath();
                }
            }
        }

        return navMeshPath;
    }
}
