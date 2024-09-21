using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public abstract class State : NetworkBehaviour
{
    [SerializeField] protected AnimationClip[] clip;

    protected Animator anim;
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected StateMachine machine;
    protected State subState => machine.subState;
    protected State currentState => machine.currentState;

    private float startTime;
    public float time => Time.time - startTime;
    public bool isCompleted = false;
    [SerializeField] private float minTimeToInvoke = 0f, maxTimeToInvoke = 0.5f;
    [SerializeField] private float blendingTime = 0.2f, offset = 0f;
    protected int animationIdx = 0;

    //States Var
    protected Vector3 alarmPos;
    protected Vector3 targetPos;
    protected float sqrDistanceToTarget;
    protected CharacterController characterController;

    public virtual void DoEnter()
    {
        if(clip.Length > 0) Invoke(nameof(PlayAnim), Random.Range(minTimeToInvoke, maxTimeToInvoke));
        startTime = Time.time;
    }

    public virtual void DoUpdate()
    {

    }

    public virtual void DoFixedUpdate()
    {
       
    }

    public virtual void DoExit()
    {
        CancelInvoke();
    }

    public void DoUpdateVariables(Vector3 target, float dst, Vector3 alarm, CharacterController contr)
    {
        targetPos = target;
        sqrDistanceToTarget = dst;
        alarmPos = alarm;
        characterController = contr;
    }


    public void Initialize(Animator an, float t, NavMeshAgent ag, Rigidbody r, StateMachine mach)
    {
        anim = an;
        startTime = t;
        agent = ag;
        rb = r;
        machine = mach;
    }

    private void PlayAnim()
    {
        if (anim.enabled)
        {
            animationIdx = Random.Range(0, clip.Length);
            anim.CrossFade(clip[animationIdx].name, blendingTime, 0, offset);
        }
    }
}
