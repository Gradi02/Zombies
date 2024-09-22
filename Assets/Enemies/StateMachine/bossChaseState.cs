using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossChaseState : State
{
    [SerializeField] private runState _runState;
    [SerializeField] private idleState _idleState;
    [SerializeField] private attackState _attackState;
    [SerializeField] private distanceAttackState _distanceAttackState;

    private float attackTime = 1f, longAttackTime = 3f;
    private float minDistanceToNormalAttack = 8;
    private float minDistanceToLongAttack = 1200;

    private float dstCooldownMin = 15, dstCooldownMax = 25;
    private float nextDst = 0;
    private bool dst = false;

    private float nextSelectState = 0;
    private float rotationSpeed = 50f;

    private float pathUpdateInterval = 0.15f;
    private float timeSinceLastPathUpdate = 0f;
    public bool isJumping { get; private set; } = false;

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
        timeSinceLastPathUpdate += Time.deltaTime;
        if (time > nextSelectState)
        {
            dst = false;
            if (sqrDistanceToTarget <= minDistanceToNormalAttack)
            {               
                nextSelectState = time + attackTime;
                if (_attackState.isCompleted || subState != _attackState)
                    machine.ChangeSubState(_attackState, true);
                agent.ResetPath();
            }
            else if(sqrDistanceToTarget > minDistanceToNormalAttack && sqrDistanceToTarget <= minDistanceToLongAttack && time > nextDst)
            {
                dst = true;
                nextDst = time + Random.Range(dstCooldownMin, dstCooldownMax);
                nextSelectState = time + longAttackTime;
                if (subState != _distanceAttackState)
                    machine.ChangeSubState(_distanceAttackState, true);

                int aIdx = _distanceAttackState.GetIdx();

                if(aIdx == 0) // 0 - throw
                {
                    StartCoroutine(IEJumpBoolean());
                }
                else
                {
                    StartCoroutine(IESpawnMinions());
                }

                agent.ResetPath();
            }
            else
            {
                machine.ChangeSubState(_runState);

                if (subState == _runState && timeSinceLastPathUpdate > pathUpdateInterval)
                {
                    if (agent.remainingDistance > agent.stoppingDistance)
                    {
                        agent.destination = targetPos;
                        timeSinceLastPathUpdate = 0f;
                    }
                }
            }
        }
        else if (sqrDistanceToTarget > 7 && !dst)
        {
            nextSelectState = 0;
        }

        if (subState == _attackState || subState == _distanceAttackState)
        {
            Vector3 directionToTarget = (targetPos - agent.transform.position).normalized;
            directionToTarget.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            // Sprawdzenie, czy rotacja jest potrzebna
            if (Vector3.Angle(agent.transform.forward, directionToTarget) > 15)
            {
                agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private IEnumerator IESpawnMinions()
    {
        yield return new WaitForSeconds(0.5f);
        int num = Random.Range(2, 5);
        for (int i = num; i >= 0; i--)
        {
            NetworkGameManager.instance.spawner.RequestBossMinion(agent.transform.position);
        }
    }

    private IEnumerator IEJumpBoolean()
    {
        isJumping = true;
        yield return new WaitForSeconds(3f);
        isJumping = false;
    }
}
