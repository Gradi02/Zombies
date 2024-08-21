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

    //States Var
    protected Vector3 alarmPos;
    protected Vector3 targetPos;
    protected float sqrDistanceToTarget;
    protected CharacterController characterController;

    public virtual void DoEnter()
    {
        if(clip.Length > 0) Invoke(nameof(PlayAnim), Random.Range(0f, 0.5f));
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
        if(anim.enabled)
            anim.CrossFade(clip[Random.Range(0, clip.Length)].name, 0.2f);
    }
}
