using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class runState : State
{
    public override void DoEnter()
    {
        agent.destination =  targetPos;
        base.DoEnter();
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
        if(agent.enabled && agent.path != null)
            agent.ResetPath(); 
        base.DoExit();
    }
}
