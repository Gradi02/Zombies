using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class distanceAttackState : State
{
    //[SerializeField] private float minDamage = 5, maxDamage = 15;
    private float breakTime;

    public override void DoEnter()
    {
        base.DoEnter();

        breakTime = time + 5f;
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
        /*if (sqrDistanceToTarget < sqrDistanceToDamage)
        {
            characterController.GetComponent<PlayerStats>().DamagePlayerServerRpc(Random.Range(minDamage, maxDamage));
        }*/
        isCompleted = true;
    }
}
