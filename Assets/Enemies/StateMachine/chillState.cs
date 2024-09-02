using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class chillState : State
{
    [SerializeField] private idleState _idleState;
    [SerializeField] private walkState _walkState;

    private float minIdleTime = 3f, maxIdleTime = 6f;
    private float minWalkTime = 15f, maxWalkTime = 35f;
    private float timeToChangeState = 0f;
    private float searchDistance = 10f;
    [SerializeField] private LayerMask mask;

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
                agent.destination = GetNewPlaceToCheck();

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
            agent.destination = GetNewPlaceToCheck();
        }
    }

    private Vector3 GetNewPlaceToCheck()
    {
        Vector3 newPos = transform.position;
        bool pathCorrect = true;
        NavMeshPath navMeshPath = new NavMeshPath();
        int iterationCount = 0;
        int maxIterations = 100;

        while (pathCorrect && (iterationCount++) < maxIterations)
        {
            Vector3 newDestRay = transform.position + new Vector3(Random.Range(-searchDistance, searchDistance), 1000, Random.Range(-searchDistance, searchDistance));
            if (Physics.Raycast(newDestRay, Vector3.down, out RaycastHit hit, 1100, mask))
            {
                if (agent.CalculatePath(agent.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    pathCorrect = false;
                    newPos = hit.point;
                }
                else
                {
                    navMeshPath = new NavMeshPath();
                }
            }
        }

        return newPos;
    }
}
