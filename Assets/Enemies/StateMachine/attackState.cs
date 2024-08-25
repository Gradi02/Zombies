using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class attackState : State
{
    private float delayToDamage = 1f;
    private float nextDamage = 0;
    private float sqrDistanceToDamage = 5;
    private float minDamage = 5, maxDamage = 15;
    private bool canDamage;

    public override void DoEnter()
    {
        base.DoEnter();

        isCompleted = false;
        canDamage = true;
        nextDamage = time + delayToDamage;
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        if(time > nextDamage && canDamage)
        {
            if(sqrDistanceToTarget < sqrDistanceToDamage)
            {
                characterController.GetComponent<PlayerStats>().DamagePlayer(Random.Range(minDamage, maxDamage));
                canDamage = false;
                isCompleted = true;
            }
        }
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
    }
}
