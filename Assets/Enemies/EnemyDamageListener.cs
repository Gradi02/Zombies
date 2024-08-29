using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageListener : MonoBehaviour
{
    [SerializeField] private attackState attackState;
    public void DealDamage()
    {
        attackState.DealDamageToPlayerServerRpc();
    }
}
