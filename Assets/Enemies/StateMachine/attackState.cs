using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class attackState : State
{
    private float sqrDistanceToDamage = 7;
    private float minDamage = 5, maxDamage = 15;
    private float breakTime;

    public override void DoEnter()
    {
        base.DoEnter();

        breakTime = time + 1.2f;
        isCompleted = false;
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
        if(time > breakTime)
            isCompleted = true;
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public void DealDamageToPlayer()
    {
        if (sqrDistanceToTarget < sqrDistanceToDamage)
        {
            characterController.GetComponent<PlayerStats>().DamagePlayer(Random.Range(minDamage, maxDamage));
        }
        isCompleted = true;
    }
}
