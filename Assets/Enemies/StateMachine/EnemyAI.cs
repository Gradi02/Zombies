using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemyAI : StateMachine
{
    public bool isDead { get; set; } = false;
    [SerializeField] private idleState _idleState;
    [SerializeField] private runState _runState;
    [SerializeField] private deathState _deathState;
    [SerializeField] private walkState _walkState;
    private Rigidbody[] ragdollRigidbodies;

    private void Start()
    {
        ChangeState(_idleState);

        agent.updatePosition = false;
        agent.updateRotation = true;

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        currentState?.DoUpdate();
        AIController();
        SyncAnimatorAndAgent();
    }

    private void FixedUpdate()
    {
        currentState?.DoFixedUpdate();
    }

    private void AIController()
    {
        switch(mainState)
        {
            case ZombieMainStates.chilling:
                {
                    ChillingStateController();
                    break;
                }
            case ZombieMainStates.alarmed:
                {

                    break;
                }
            case ZombieMainStates.chasing:
                {
                    ChaseStateController();
                    break;
                }
            case ZombieMainStates.critical:
                {

                    break;
                }
        }
    }

    [Header("Chilling State Variables")]
    private float minIdleTime = 3f, maxIdleTime = 6f;
    private float minWalkTime = 5f, maxWalkTime = 15f;
    private float timeToChangeState = 0f;

    private void ChillingStateController()
    {
        if (Time.time > timeToChangeState)
        {
            if (currentState == _idleState)
            {
                ChangeState(_walkState);

                agent.enabled = true;
                agent.destination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                
                timeToChangeState = Time.time + Random.Range(minWalkTime, maxWalkTime);
            }
            else if (currentState == _walkState)
            {
                ChangeState(_idleState);

                agent.ResetPath();
                agent.enabled = false;
                
                timeToChangeState = Time.time + Random.Range(minIdleTime, maxIdleTime);
            }
        }

        if(agent.enabled && agent.remainingDistance < agent.radius)
        {
            agent.destination = transform.position + new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
        }
    }

    [Header("Chase State Variables")]
    private float minAttackTime = 3f, maxAttackTime = 6f;
    private Transform player;
    private Vector2 velocity;
    private Vector2 SmoothDeltaPosition;

    private void ChaseStateController()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.enabled = true;
        agent.destination = player.position;
        ChangeState(_runState);
    }



    void OnAnimatorMove()
    {
        Vector3 position = animator.rootPosition;

        if(!isDead) position.y = agent.nextPosition.y;
        
        transform.position = position;
        agent.nextPosition = transform.position;
    }

    void SyncAnimatorAndAgent()
    {
        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
        worldDeltaPosition.y = 0;
        float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
        float deltaMagnitude = worldDeltaPosition.magnitude;
        if (deltaMagnitude > agent.radius / 4f)
        {
            transform.position = Vector3.Lerp(
                animator.rootPosition,
                agent.nextPosition,
                smooth
            );
        }
    }
    public void DeathState()
    {
        isDead = true;
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }
        ChangeState(_deathState, true);
    }
}
