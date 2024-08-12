using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State : MonoBehaviour
{
    [SerializeField] private AnimationClip[] clip;

    private Animator anim;
    private float startTime;
    protected NavMeshAgent agent;

    public float time => Time.time - startTime;

    public virtual void DoEnter()
    {
        Invoke(nameof(PlayAnim), Random.Range(0f, 0.1f));
    }

    public virtual void DoUpdate()
    {

    }

    public virtual void DoFixedUpdate()
    {

    }

    public virtual void DoExit()
    {
        
    }


    public void Initialize(Animator an, float t, NavMeshAgent ag)
    {
        anim = an;
        startTime = t;
        agent = ag;
    }

    private void PlayAnim()
    {
        anim.CrossFade(clip[Random.Range(0, clip.Length)].name, 0.2f);
    }
}
