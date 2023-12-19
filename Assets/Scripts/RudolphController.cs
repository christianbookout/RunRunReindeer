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
    public float loseInterestTime = 5f;
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
                 Vector3.Distance(moveLocation, player.transform.position) > curRadius)
        {
            float smell = Mathf.Pow(smellExponentialDecay, distance);

            curRadius = smell * smellRadius;
            // Find a safe position for Rudolph to move to
            do {
                Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * curRadius;
                moveLocation = player.transform.position + randomPoint;
            } while (!NavMesh.SamplePosition(moveLocation, out NavMeshHit _, 1f, NavMesh.AllAreas));
        }
        agent.SetDestination(moveLocation);
        
    }

    float hideTime = 0f;

    void ChasePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool seesPlayer = Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit _, sightDistance);
        // Check if Rudolph can see the player
        if (!seesPlayer && hideTime < loseInterestTime)
        {
            hideTime += Time.deltaTime;
        }
        // If Rudolph hasn't seen the player for a given amount of time, Rudolph should go back to hunting
        else if (!seesPlayer && hideTime >= loseInterestTime)
        {
            currentState = State.Hunting;
        }
        else {
            hideTime = 0f;
        }

        if (distance > loseSightDistance )
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
