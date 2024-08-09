using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    [SerializeField] private AnimationClip clip;

    private Animator anim;
    private float startTime;

    public float time => Time.time - startTime;

    public virtual void DoEnter()
    {
        anim.Play(clip.name);
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


    public void Initialize(Animator an, float t)
    {
        anim = an;
        startTime = t;
    }
}
