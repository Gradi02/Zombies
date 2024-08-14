using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chaseState : State
{
    [SerializeField] private runState _runState;
    [SerializeField] private idleState _idleState;
    [SerializeField] private attackState _attackState;

    private float attackTime = 1.15f;
    private Transform target;
    private float maxDistanceToTarget = 400;
    private float minDistanceToAttack = 5;
    private float nextSelectState = 0;
    private float rotationSpeed = 50f;
    private float angleToTarget;

    public override void DoEnter()
    {
        base.DoEnter();

        agent.enabled = true;
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
    }


    private void SelectSubState()
    {
        if (target == null)
        {
            SelectTarget();
            machine.ChangeSubState(_idleState);
            return;
        }
        else if (time > nextSelectState)
        {
            float dstToTarget = (agent.transform.position - target.position).sqrMagnitude;

            if (dstToTarget <= minDistanceToAttack && angleToTarget < 45f)
            {
                nextSelectState = time + attackTime;
                machine.ChangeSubState(_attackState, true);
                agent.ResetPath();
            }
            else
            {
                machine.ChangeSubState(_runState);
                agent.destination = target.position;
            }
        }

        if(subState == _attackState)
        {
            Vector3 directionToTarget = (target.position - agent.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);
        }
    }

    private void SelectTarget()
    {
        if(target == null && players != null)
        {
            GameObject nearestObject = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject p in players)
            {
                float sqrDst = (agent.transform.position - p.transform.position).sqrMagnitude;

                if (sqrDst < shortestDistance && sqrDst <= maxDistanceToTarget)
                {
                    shortestDistance = sqrDst;
                    nearestObject = p;
                }
            }

            if (nearestObject != null)
            {
                target = nearestObject.transform;
                machine.ChangeSubState(_runState);
            }
        }
        else if(players != null)
        {
            float sqrDst = (agent.transform.position - target.position).sqrMagnitude;
            if (sqrDst > maxDistanceToTarget) target = null;
        }
    }

    bool IsPlayingAnyAnimation()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime < 1.0f;
    }
}

