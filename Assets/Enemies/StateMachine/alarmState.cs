using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class alarmState : State
{
    [SerializeField] private walkState _walkState;
    private float minInterrestTime = 30f, maxInterrestTime = 40f;
    private float timeToComplete;
    private float searchDistance = 15f;
    [SerializeField] private LayerMask mask;

    public override void DoEnter()
    {
        base.DoEnter();
        timeToComplete = time + Random.Range(minInterrestTime, maxInterrestTime);

        agent.enabled = true;
        machine.ChangeSubState(_walkState);
        agent.destination = GetNewPlaceToCheck();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        if (time > timeToComplete || sqrDistanceToTarget < 5)
            isCompleted = true;
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();

        isCompleted = false;
    }

    private Vector3 GetNewPlaceToCheck()
    {
        Vector3 newPos = Vector3.zero;
        bool pathCorrect = true;
        NavMeshPath navMeshPath = new NavMeshPath();
        int iterationCount = 0;
        int maxIterations = 100;

        while (pathCorrect && (iterationCount++) < maxIterations)
        {
            Vector3 newDestRay = alarmPos + new Vector3(Random.Range(-searchDistance, searchDistance), 1000, Random.Range(-searchDistance, searchDistance));
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
