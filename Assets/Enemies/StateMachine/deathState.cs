using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class deathState : State
{
    public override void DoEnter()
    {
        base.DoEnter();

        if (IsHost)
            DeathEffectClientRpc();
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
