using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State : MonoBehaviour
{
    [SerializeField] protected AnimationClip[] clip;

    protected Animator anim;
    protected NavMeshAgent agent;
    protected Rigidbody rb;
    protected StateMachine machine;
    protected State subState => machine.subState;

    private float startTime;
    public float time => Time.time - startTime;


    protected GameObject[] players;

    public virtual void DoEnter()
    {
        if(clip.Length > 0) Invoke(nameof(PlayAnim), Random.Range(0f, 0.1f));
        startTime = Time.time;
    }

    public virtual void DoUpdate()
    {

    }

    public virtual void DoFixedUpdate()
    {
        try
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }
        catch
        {
            players = null;
        }
    }

    public virtual void DoExit()
    {
        
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
