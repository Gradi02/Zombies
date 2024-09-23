using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class distanceAttackState : State
{
    [SerializeField] private bossChaseState _bossChaseState;
    private float breakTime;

    public override void DoEnter()
    {
        base.DoEnter();

        breakTime = time + 5f;
        isCompleted = false;
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
        if(time > breakTime)
            isCompleted = true;
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
    }

    public override void PlayAnim()
    {
        if (anim.enabled)
        {
            animationIdx = Random.Range(0, clip.Length);
            _bossChaseState.SelectAttack(animationIdx);
            anim.CrossFade(clip[animationIdx].name, blendingTime, 0, offset);
        }
    }
}
