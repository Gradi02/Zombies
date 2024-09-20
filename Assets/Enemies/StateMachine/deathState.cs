using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class deathState : State
{
    [SerializeField] private float timeToDeath = 0;
    public override void DoEnter()
    {
        base.DoEnter();

        if (IsHost)
            Invoke(nameof(DeathEffectClientRpc), timeToDeath);
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

    [ClientRpc]
    private void DeathEffectClientRpc()
    {
        anim.enabled = false;
        agent.velocity = Vector3.zero;
        agent.enabled = false;
    }
}
