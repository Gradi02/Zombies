using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chaseState : State
{
    [SerializeField] private runState _runState;
    [SerializeField] private idleState _idleState;
    [SerializeField] private attackState _attackState;

    private float attackTime = 1.15f;
    private float minDistanceToAttack = 5;

    private float nextSelectState = 0;
    private float rotationSpeed = 50f;
    private float angleToTarget;

    public override void DoEnter()
    {
        base.DoEnter();

        agent.enabled = true;
        nextSelectState = 0;
        agent.destination = targetPos;
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
        agent.enabled = false;
    }


    private void SelectSubState()
    {
        if (time > nextSelectState)
        {
            if (sqrDistanceToTarget <= minDistanceToAttack)
            {
                nextSelectState = time + attackTime;
                if(_attackState.isCompleted || subState != _attackState)
                    machine.ChangeSubState(_attackState, true);
                agent.ResetPath();
            }
            else
            {
                machine.ChangeSubState(_runState);

                if(subState == _runState)
                    agent.destination = targetPos;
            }
        }

        if(subState == _attackState)
        {
            Vector3 directionToTarget = (targetPos - agent.transform.position).normalized;
            directionToTarget.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);
        }
    }
}

