using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyDamageListener : NetworkBehaviour
{
    [SerializeField] private attackState attackState;
    public void DealDamage()
    {
        if(IsServer)
            attackState.DealDamageToPlayer();
    }
}
