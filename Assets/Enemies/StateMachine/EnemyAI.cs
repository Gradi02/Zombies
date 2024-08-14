using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemyAI : StateMachine
{
    public bool isDead { get; set; } = false;

    [SerializeField] private chillState _chillState;
    [SerializeField] private alarmState _alarmState;
    [SerializeField] private chaseState _chaseState;
    [SerializeField] private critState _critState;
    [SerializeField] private deathState _deathState;

    private Rigidbody[] ragdollRigidbodies;

    private void Start()
    {
        ChangeState(_chaseState);

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
        SelectMainState();
        SyncAnimatorAndAgent();
    }

    private void FixedUpdate()
    {
        currentState?.DoFixedUpdate();
    }

    private void SelectMainState()
    {
        //select state
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
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }
        ChangeState(_deathState, true);
    }
}
