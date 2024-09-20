using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public enum NPCState_AT
{
    Evade,
    Patrol,
    Attack,
    Flee,
    //new state
    Idle

}

public class NPCController : MonoBehaviour
{
    private NPCState_AT currentState;
    public Transform[] patrolPoints;  // Array of points for patrolling
    private int patrolIndex = 0;
    public Transform enemy;  // Reference to the enemy
    public float attackRange = 2f;  // Range for attacking
    public float detectionRange = 5f;  // Range to detect threats
    public float speed = 2f;

    private void Update()
    {
        // FSM logic handled through switch-case based on current state
        switch (currentState)
        {
            case NPCState_AT.Idle:
                PerformIdle();
                break;

            case NPCState_AT.Patrol:
                PerformPatrol();
                break;

            case NPCState_AT.Evade:
                PerformEvade();
                break;

            case NPCState_AT.Attack:
                PerformAttack();
                break;

            case NPCState_AT.Flee:
                PerformFlee();
                break;

            default:
                Debug.LogError("Unknown state encountered: " + currentState);
                break;
        }
    }

    private void ChangeState(NPCState_AT newState)
    {
        currentState = newState;
        Debug.Log("State changed to: " + currentState);
    }

    private void PerformIdle()
    {
        if (NearbyEntity())
        {
            ChangeState(NPCState_AT.Patrol);
        }
    }

    private void PerformPatrol()
    {
        PatrolPath();
        if (DetectThreat())
        {
            if (IsStrongerThanThreat())
            {
                ChangeState(NPCState_AT.Attack);
            }
            else
            {
                ChangeState(NPCState_AT.Flee);
            }
        }
    }

    private void PerformEvade()
    {
        ExecuteEvade();
        if (IsSafe())
        {
            ChangeState(NPCState_AT.Patrol);
        }
    }

    private void PerformAttack()
    {
        ExecuteAttack();
        if (!IsStrongerThanThreat())
        {
            ChangeState(NPCState_AT.Flee);
        }
    }

    private void PerformFlee()
    {
        ExecuteFlee();
        if (IsSafe())
        {
            ChangeState(NPCState_AT.Idle);
        }
    }

    // Helper methods
    private bool NearbyEntity() => true; // Simplified
    private void PatrolPath() { /* Patrol logic */ }
    private bool DetectThreat() => true; // Simplified threat detection
    private bool IsStrongerThanThreat() => true; // Compare strength with the threat
    private void ExecuteAttack() { /* Attack logic */ }
    private void ExecuteFlee() { /* Flee logic */ }
    private void ExecuteEvade() { /* Evade logic */ }
    private bool IsSafe() => true; // Safe condition check

}



