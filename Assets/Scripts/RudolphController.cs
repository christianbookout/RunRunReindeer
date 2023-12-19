using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RudolphController : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;
    public float attackDistance = 1.5f;
    // The distance Rudolph loses the player from their sight, and must rely on scent.
    public float loseSightDistance = 15f;
    public float sightDistance = 10f;
    public float hearsRunningDistance = 15f;
    public float hearsWalkingDistance = 10f;
    public float smellExponentialDecay = 0.5f;
    public float smellRadius = 1000f;
    public float smellUpdateSeconds = 3f;
    private NavMeshAgent agent;
    
    public enum State
    {
        Hunting,
        Chasing,
        Attacking
    }

    private State currentState = State.Hunting;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerController = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Hunting:
                HuntPlayer();
                break;
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                AttackPlayer();
                break;
        }
    }

    Vector3 moveLocation = Vector3.zero;
    float curRadius = 0f;
    float lastPositionUpdate = 0f;

    void HuntPlayer()
    {
        // Rudolph should hunt down the player by "searching" for them. Rudolph should "smell" them based on how far they are, and should walk towards a random point close to the player (depending on how well Rudolph can smell them). If Rudolph gets close enough to the player, Rudolph should start chasing them. If the player gets outside of the smell range, a new random point should be chosen.
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool seesPlayer = Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit _, sightDistance);
        bool hearsPlayer = (playerController.IsRunning && distance < hearsRunningDistance) || (playerController.IsWalking && distance < hearsWalkingDistance);
        if (seesPlayer || hearsPlayer)
        {
            currentState = State.Chasing;
            moveLocation = player.transform.position;
            curRadius = 0f;
        }
        else if (Vector3.Distance(transform.position, moveLocation) < 3f || 
                 Vector3.Distance(moveLocation, player.transform.position) > curRadius ||
                 lastPositionUpdate + smellUpdateSeconds < Time.time)
        {
            float smell = Mathf.Pow(smellExponentialDecay, distance);

            curRadius = smell * smellRadius;
            Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * curRadius;
            moveLocation = player.transform.position + randomPoint;

            lastPositionUpdate = Time.time;
        }
        agent.SetDestination(moveLocation);
        
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, curRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(moveLocation, 1f);
    }

    void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > loseSightDistance)
        {
            currentState = State.Hunting;
        }
        else if (distance < attackDistance)
        {
            currentState = State.Attacking;
        }
        else
        {
            agent.SetDestination(player.transform.position);
        }
    }

    void AttackPlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance > attackDistance)
        {
            currentState = State.Chasing;
        }
        else
        {
            agent.SetDestination(player.transform.position);
            // TODO kill player here
        }
    }
}
