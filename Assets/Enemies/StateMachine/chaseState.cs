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
    private float maxPredictorDistance = 10;
    private float minPDsqr, maxPDsqr;

    private float nextSelectState = 0;
    private float rotationSpeed = 50f;
    private float angleToTarget;
    private bool predictor = false;
    private float maxPrediction;

    public override void DoEnter()
    {
        base.DoEnter();

        minPDsqr = minDistanceToAttack * minDistanceToAttack;
        maxPDsqr = maxPredictorDistance * maxPredictorDistance;
        maxPrediction = Random.Range(1.0f, 3.0f);

        predictor = Random.value < 0.5f;
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

        isCompleted = false;
        agent.ResetPath();
        agent.enabled = false;
    }


    private void SelectSubState()
    {
        if (time > nextSelectState)
        {
            if (sqrDistanceToTarget <= minDistanceToAttack && angleToTarget < 45f)
            {
                nextSelectState = time + attackTime;
                machine.ChangeSubState(_attackState, true);
                agent.ResetPath();
            }
            else if(predictor && sqrDistanceToTarget > maxPredictorDistance && characterController != null)
            {
                if(subState != _runState)
                    machine.ChangeSubState(_runState);

                Vector3 playerDirection = characterController.velocity.normalized;
                float playerSpeed = characterController.velocity.magnitude;

                float predictionTime = Mathf.Lerp(0.1f, maxPrediction, Mathf.InverseLerp(minPDsqr, maxPDsqr, sqrDistanceToTarget));
                Vector3 predictedPosition = targetPos + playerDirection * playerSpeed * predictionTime;

                agent.destination = predictedPosition;
            }
            else
            {
                machine.ChangeSubState(_runState);
                agent.destination = targetPos;
            }
        }

        if(subState == _attackState)
        {
            Vector3 directionToTarget = (targetPos - agent.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            angleToTarget = Vector3.Angle(agent.transform.forward, directionToTarget);
        }
    }
}

