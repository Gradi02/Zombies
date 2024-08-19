using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class reactionState : State
{
    private float waitTime = 1.5f, toComplete;
    [SerializeField] private AnimationClip[] clips2;
    public override void DoEnter()
    {
        base.DoEnter();
        agent.enabled = false;
        toComplete = time + waitTime;
        machine.ChangeSubState(null);

        if (anim.enabled)
            anim.CrossFade(clips2[Random.Range(0, clip.Length)].name, 0.2f, 0, 0.2f);
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        if (time > toComplete)
            isCompleted = true;
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
        agent.enabled = true;
    }
}
