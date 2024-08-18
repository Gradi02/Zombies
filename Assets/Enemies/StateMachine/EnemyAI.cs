using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

public class EnemyAI : StateMachine
{
    public bool isDead { get; set; } = false;

    [Header("States")]
    [SerializeField] private chillState _chillState;
    [SerializeField] private alarmState _alarmState;
    [SerializeField] private chaseState _chaseState;
    [SerializeField] private critState _critState;
    [SerializeField] private huntState _huntState;
    [SerializeField] private deathState _deathState;

    [Header("Sub States")]
    [SerializeField] private idleState _idleState;
    [SerializeField] private walkState _walkState;
    [SerializeField] private runState _runState;
    [SerializeField] private attackState _attackState;

    private Rigidbody[] ragdollRigidbodies;

    //States Var
    private float maxDistanceToTarget = 400;
    public LayerMask obstacleMask;

    private GameObject[] players = null;
    private Transform target;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 alarmPos = Vector3.zero;
    private float sqrDistanceToTarget = 0;
    private bool seePlayer = false;
    private Vector3 eyeLevel = new Vector3(0, 1, 0);

    private void Start()
    {
        ChangeState(_chillState);

        agent.updatePosition = false;
        agent.updateRotation = false;

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    private void Update()
    {
        if (!IsHost) return;

        currentState?.DoUpdate();
        subState?.DoUpdate();

        SetVariables();
        foreach (State s in states)
            s.DoUpdateVariables(targetPos, sqrDistanceToTarget, alarmPos);
        
        SelectMainState();
        SyncAnimatorAndAgent();
    }

    private void FixedUpdate()
    {
        if (!IsHost) return;
        currentState?.DoFixedUpdate();
        subState?.DoFixedUpdate();
    }

    private void SelectMainState()
    {
        //select state
        if (currentState == _chillState)
        {
            //przejœcie do alarmu
            if(alarmPos != Vector3.zero)
            {
                ChangeState(_alarmState);
            }
            //przejœcie do chase
            else if(target != null && seePlayer)
            {
                ChangeState(_chaseState);
            }
        }
        else if(currentState == _alarmState)
        {
            //przejœcie do idle
            if(currentState.isCompleted)
            {
                alarmPos = Vector3.zero;
                ChangeState(_chillState);
            }
            //przejœcie do chase
            else if (target != null && seePlayer)
            {
                alarmPos = Vector3.zero;
                ChangeState(_chaseState);
            }
        }
        else if(currentState == _chaseState)
        {
            //przejœcie do hunt state
            if (target == null)
            {
                ChangeState(_huntState);
            }
        }
        else if(currentState == _huntState)
        {
            //przejœcie do chillku
            if(currentState.isCompleted)
            {
                ChangeState(_chillState);
            }
            //przejœcie do chase
            else if (target != null && seePlayer)
            {
                ChangeState(_chaseState);
            }
        }
    }
    private void SetVariables()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        SelectTarget();
        if (target != null)
        {
            Vector3 dir = (targetPos + eyeLevel) - (agent.transform.position + eyeLevel);
            sqrDistanceToTarget = dir.sqrMagnitude;

            if (sqrDistanceToTarget < maxDistanceToTarget)
            {
                Vector3 origin = agent.transform.position + eyeLevel;

                if (!Physics.Raycast(origin, dir.normalized, Mathf.Sqrt(sqrDistanceToTarget), obstacleMask))
                {
                    //Debug.DrawLine(origin, targetPos + eyeLevel, Color.green);
                    seePlayer = true;
                }
                else
                {
                    //Debug.DrawLine(origin, targetPos + eyeLevel, Color.red);
                    seePlayer = false;
                }
            }
            else
            {
                seePlayer = false;
            }
            
            targetPos = target.position;
        }
    }
    private void SelectTarget()
    {
        if (target == null && players != null)
        {
            GameObject nearestObject = null;
            float shortestDistance = Mathf.Infinity;

            foreach (GameObject p in players)
            {
                float sqrDst = (agent.transform.position - p.transform.position).sqrMagnitude;

                if (sqrDst < shortestDistance && sqrDst <= maxDistanceToTarget)
                {
                    shortestDistance = sqrDst;
                    nearestObject = p;
                }
            }

            if (nearestObject != null)
            {
                target = nearestObject.transform;
                targetPos = target.position;
            }
        }
        else if (players != null)
        {
            float sqrDst = (agent.transform.position - target.position).sqrMagnitude;
            if (sqrDst > maxDistanceToTarget) target = null;
        }
    }

    void OnAnimatorMove()
    {
        if (!IsHost) return;
        Vector3 position = animator.rootPosition;

        if(!isDead) position.y = agent.nextPosition.y;
        
        transform.position = position;
        agent.nextPosition = transform.position;

        var turnTowardNavSteeringTarget = agent.steeringTarget;
        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
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

    public void AlarmEnemy(Vector3 ap)
    {
        if (currentState == _chillState)
            alarmPos = ap;
        else if(currentState == _alarmState)
        {
            alarmPos = ap;
            currentState.DoUpdateVariables(targetPos, sqrDistanceToTarget, alarmPos);
            ChangeState(_alarmState, false, true);
        }
        else if(currentState == _huntState)
        {
            targetPos = ap;
        }
    }

    [ClientRpc]
    public void DeathStateClientRpc()
    {
        isDead = true;
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if(IsHost)
            ChangeState(_deathState, true);
    }
}
