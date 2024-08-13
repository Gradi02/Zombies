using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathState : State
{
    public override void DoEnter()
    {
        base.DoEnter();

        anim.enabled = false;
        agent.velocity = Vector3.zero;
        agent.enabled = false;
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
}
