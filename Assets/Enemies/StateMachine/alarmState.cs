using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class alarmState : State
{
    [SerializeField] private walkState _walkState;
    private float minInterrestTime = 10f, maxInterrestTime = 20f;
    private float timeToComplete;

    public override void DoEnter()
    {
        base.DoEnter();
        timeToComplete = time + Random.Range(minInterrestTime, maxInterrestTime);

        agent.enabled = true;
        machine.ChangeSubState(_walkState);
        agent.destination = alarmPos;
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
}
