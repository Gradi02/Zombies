using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Enemy : NetworkBehaviour, IDamage
{
    public float hp = 100;
    public GameObject leftArm, rightArm;
    public GameObject leftLeg, rightLeg;
    public GameObject head;


    public void TakeDamage(int amount)
    {
        RequestDamageEntityServerRpc(amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDamageEntityServerRpc(int amount)
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
        GetComponent<EnemyAI>().DeathState();



       /* GetComponent<NetworkObject>().Despawn();
        DestroyEntityClientRpc();*/
    }

/*    [ClientRpc]
    private void DestroyEntityClientRpc()
    {
        Destroy(gameObject);
    }*/
}
