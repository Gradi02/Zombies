using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runState : State
{
    [SerializeField] private chaseState _chaseState;
    public override void DoEnter()
    {
        base.DoEnter();

        AlarmNearEnemies();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
    }


    public LayerMask enemyLayer;
    private void AlarmNearEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100, enemyLayer);

        foreach (Collider hitCollider in hitColliders)
        {
            EnemyAI enemyScript = hitCollider.GetComponent<EnemyAI>();

            if (enemyScript != null && currentState == _chaseState)
            {
                enemyScript.AlarmEnemy(targetPos);
            }
        }
    }
}
