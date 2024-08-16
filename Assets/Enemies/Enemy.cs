using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour, IDamage
{
    public float hp = 100;



    public void TakeDamage(float amount)
    {
        RequestDamageEntityServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDamageEntityServerRpc(float amount)
    {
        hp -= amount;
        if(hp <= 0)
        {
            RequestKillEntityServerRpc();
        }
    }

    [ServerRpc]
    private void RequestKillEntityServerRpc()
    {
        GetComponent<EnemyAI>().DeathStateClientRpc();
    }
}
