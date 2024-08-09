using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class runState : State
{
    private NavMeshAgent agent;
    private GameObject[] players;

    public override void DoEnter()
    {
        base.DoEnter();
        Debug.Log("run");

        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public override void DoUpdate()
    {
        base.DoUpdate();

        agent.destination = players[0].transform.position;
    }

    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
    }

    public override void DoExit()
    {
        base.DoExit();
    }


    public void SetUpState(NavMeshAgent a)
    {
        agent = a;
    }
}
