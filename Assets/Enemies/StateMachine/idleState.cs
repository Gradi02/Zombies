using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idleState : State
{


    public override void DoEnter()
    {
        base.DoEnter();
        Debug.Log("idle");
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
