using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class NPCController2 : MonoBehaviour
{
    StateMachine stateMachine;
    StateMachine.State patrolState;
    StateMachine.State attackState;
    StateMachine.State runAwayState;

    public GameObject goOpponent;

    [Header("Temporary (Mock Variables)")]
    [SerializeField] bool bSafe;
    [SerializeField] int HealthPoints = 100;
    [SerializeField] int EnemyHealthPoints = 50;
    [SerializeField] float distanceCutOff = 5;
    private float sqrDistanceCutoff;
    public float speed = 5f; // m/s

    public float originalY;

    public Transform[] Waypoints;
    public int currentWaypointIndex; //
    public Transform currentWaypoint; // for convenience
    public Transform nextWaypoint; //

    public float TOL = 0.001f; // when do we consider NPC is IN the next WP.
    Func<int, int, int> nextWaypointIndex = (i, NwP) => (i + 1) % NwP; //
    public int NumberOfWaypoints;

    // rotation speed
    float rotSpeed = 60f; // 60 degrees per sec

    // Start is called before the first frame update
    void Start()
    {

        NumberOfWaypoints = Waypoints.Length;
        originalY = this.transform.position.y;
        currentWaypointIndex = 0;
        currentWaypoint = Waypoints[currentWaypointIndex];
        nextWaypoint = Waypoints[nextWaypointIndex(currentWaypointIndex, NumberOfWaypoints)];

        sqrDistanceCutoff = distanceCutOff * distanceCutOff;
        stateMachine = new StateMachine();

        //Patrol
        patrolState = stateMachine.CreateState("Patrol");
        patrolState.OnEnter = delegate
        {
            Debug.Log("Patrol.OnEnter...");
        };
        patrolState.OnExit = delegate
        {
            Debug.Log("Patrol.OnExit...");
        };
        patrolState.OnFrame = PatrolOnFrame;

        //Attack
        attackState = stateMachine.CreateState("Attack");
        attackState.OnEnter = delegate
        {
            Debug.Log("Attack.OnEnter...");
        };
        attackState.OnExit = delegate
        {
            Debug.Log("Attack.OnExit...");
        };
        attackState.OnFrame = delegate
        {
            Debug.Log("Attack.OnFrame...");
        };

        //Run Away
        runAwayState = stateMachine.CreateState("RunAway");
        runAwayState.OnEnter = delegate
        {
            Debug.Log("RunAway.OnEnter...");
        };
        runAwayState.OnExit = delegate
        {
            Debug.Log("RunAway.OnExit...");
        };
        runAwayState.OnFrame = delegate
        {
            Debug.Log("RunAway.OnFrame...");
            if (Safe())
            {
                stateMachine.TransitionTo(patrolState);
            }
            EvadeEnemy();
        };

        stateMachine.ready = true;
    }

    void PatrolOnFrame()
    {
        Debug.Log("Patrol.OnFrame...");
        if (Threatened())
        {
            if (StrongerThanEnemy())
            {
                //ChangeState(NPCState_AT.Attack);
                stateMachine.TransitionTo(attackState);
            }
            else
            {
                stateMachine.TransitionTo(runAwayState);
            }
        }
        FollowPatrolPath();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    // Evade Logic
    private Boolean Safe()
    {
        // return bSafe; //MOCK
        Vector3 headingNPC2Opponent = goOpponent.transform.position - this.transform.position;
        // float distance = headingNPC2Opponent.magnitude;
        float sqrDistance = headingNPC2Opponent.sqrMagnitude;
        return sqrDistanceCutoff > sqrDistance;
    }

    private void EvadeEnemy()
    {
        Debug.Log("EvadeEnemy"); //MOCK
        Vector3 headingNPC2Opponent = goOpponent.transform.position - this.transform.position;

        // float distance = headingNPC2Opponent.magnitude;
        float sqrDistance = headingNPC2Opponent.sqrMagnitude;
        headingNPC2Opponent.Normalize(); // make it a unit vector
        transform.Translate(-headingNPC2Opponent * speed * Time.deltaTime);
    }

    private void FollowPatrolPath()
    {
        Vector3 heading = currentWaypoint.position - this.transform.position;
        float distance = heading.magnitude;
        if (distance < TOL)
        {
            currentWaypointIndex = nextWaypointIndex(currentWaypointIndex, NumberOfWaypoints);
            currentWaypoint = Waypoints[currentWaypointIndex];
            nextWaypoint = Waypoints[nextWaypointIndex(currentWaypointIndex, NumberOfWaypoints)];
        }
        else
        {
            heading.Normalize(); // make it a unit vector
            transform.Translate(heading * speed * Time.deltaTime);
        }
    }


    private bool StrongerThanEnemy()
    {
        return HealthPoints > EnemyHealthPoints;
    }

    private bool Threatened()
    {
        return !bSafe;
    }
}