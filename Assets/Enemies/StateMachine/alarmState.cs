using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class alarmState : State
{
    [SerializeField] private walkState _walkState;
    private float minInterrestTime = 10f, maxInterrestTime = 20f;
    private float timeToComplete;
    private float searchDistance = 15f;
    [SerializeField] private LayerMask mask;

    public override void DoEnter()
    {
        base.DoEnter();
        timeToComplete = time + Random.Range(minInterrestTime, maxInterrestTime);

        agent.enabled = true;
        machine.ChangeSubState(_walkState);
        agent.path = GetNewPlaceToCheck();
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
        agent.ResetPath();
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
