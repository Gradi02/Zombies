using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class deathState : State
{
    [SerializeField] private float timeToDeath = 0;
    [SerializeField] private ParticleSystem ps;
    [SerializeField] private BossEnemyAI ai;
    public override void DoEnter()
    {
        base.DoEnter();

        if (IsHost)
        {
            if(ai != null)
                Invoke(nameof(MutantDeathEffectServerRpc), timeToDeath);
            else
                Invoke(nameof(DeathEffectClientRpc), timeToDeath);
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void MutantDeathEffectServerRpc()
    {
        DeathEffectClientRpc();

        if (ai.rewardItem != null)
        {
            GameObject g = Instantiate(ai.rewardItem, ai.transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            g.GetComponent<NetworkObject>().Spawn();
        }

        ai.gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(ai.gameObject);
    }

    [ClientRpc]
    private void DeathEffectClientRpc()
    {
        anim.enabled = false;
        agent.velocity = Vector3.zero;
        agent.enabled = false;

        if (ps != null)
        {
            GameObject g = Instantiate(ps, ai.transform.position + Vector3.up, Quaternion.identity).gameObject;
            Destroy(g, 5);
        }
    }
}
