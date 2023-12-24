using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RudolphController : MonoBehaviour
{
    public GameObject player;
    [Header("Audio")]
    public AudioSource lostPlayerRoar;
    public AudioSource locatedRoar;
    public AudioSource[] searchingRoars;
    public AudioSource footsteps;
    public AudioSource jumpscare;
    public float footstepsWalkPitch = 1f;
    public float footstepsRunPitch = 1.5f;
    public float searchingRoarFrequency = 30f;
    public float searchingRoarFrequencyVariance = 15f;
    public float searchingRoarPitch = 1f;
    public float searchingRoarPitchVariance = 0.2f;
    [Header("AI Movement")]
    public float attackDistance = 1.5f;
    public float viewAngle = 120f;
    // The distance Rudolph loses the player from their sight, and must rely on scent.
    public float loseSightDistance = 15f;
    public float sightDistance = 10f;
    public float hearsRunningDistance = 15f;
    public float hearsWalkingDistance = 10f;
    public float loseInterestTime = 5f;
    // The time Rudolph will take after losing interest and before finding a new point to search by scent.
    public float walkAwayTime = 2f;
    [Header("AI Smell")]
    public float smellExponentialDecay = 0.5f;
    public float randomMovePointRadiusOffset = 5f;
    public float maxSmellRadius = 50f;
    public float minSmellRadius = 25f;
    public float smellUpdateSeconds = 3f;
    private NavMeshAgent agent;
    private PlayerController playerController;
    
    public enum State
    {
        Hunting,
        Chasing,
        Attacking
    }

    private State currentState = State.Hunting;
    private Animator anim;
    private bool canIdle = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerController = player.GetComponent<PlayerController>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Playing) return;
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
        AddGravity();
        WalkSound();
        SearchingRoar();
        HandleAnimations();
    }

    private void HandleAnimations() {
        if (currentState == State.Chasing) {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
        else if (currentState == State.Hunting) {
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
        else if (currentState == State.Attacking) {
            anim.SetTrigger("attack");
        }
        
        if (canIdle && walkTime > 0 ) {
            anim.SetTrigger("idle");
            // Only idle once
            canIdle = false;
        }
    }

    private void WalkSound() {
        if (currentState == State.Chasing) {
            footsteps.pitch = footstepsRunPitch;
        }
        else {
            footsteps.pitch = footstepsWalkPitch;
        }
        
        if (agent.velocity.magnitude > 0.1f && !footsteps.isPlaying) {
            footsteps.Play();
        }
        else if (agent.velocity.magnitude <= 0.1f && footsteps.isPlaying) {
            footsteps.Stop();
        }
    }
    
    private void AddGravity() {
        var gravity = Physics.gravity;
        agent.Move(gravity * Time.deltaTime);
    }

    bool HearsPlayer() {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool hearsPlayer = (playerController.IsRunning && distance < hearsRunningDistance) || (playerController.IsWalking && distance < hearsWalkingDistance);
        return hearsPlayer;
    }

    bool SeesPlayer()
    {
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
        {
            Physics.Raycast(transform.position, player.transform.position - transform.position, out RaycastHit hit, sightDistance);
            return hit.collider != null && hit.collider.gameObject == player;
        }
        return false;
    }

    // Raycast up/down to find the ground
    Vector3 FindGround(Vector3 startPos) {
        Vector3 groundPos = startPos;
        if (Physics.Raycast(startPos, Vector3.down, out RaycastHit hit1, 100f)) {
            groundPos = hit1.point;
        }
        else if (Physics.Raycast(startPos, Vector3.up, out RaycastHit hit2, 100f)) {
            groundPos = hit2.point;
        }
        return groundPos;
    }

    float nextRoarTime = 0f; 
    void SearchingRoar() {
        if (currentState != State.Hunting) {
            nextRoarTime = 0f;
            return;
        }

        float calculateRoarTime() {
            return Time.time + UnityEngine.Random.Range(searchingRoarFrequency - searchingRoarFrequencyVariance, searchingRoarFrequency + searchingRoarFrequencyVariance);
        }

        // When the previous state was chasing, Rudolph's next roar should be delayed.
        if (nextRoarTime == 0f) {
            nextRoarTime = calculateRoarTime();
        }

        if (Time.time > nextRoarTime) {
            var searchingRoar = searchingRoars[UnityEngine.Random.Range(0, searchingRoars.Length)];
            nextRoarTime = calculateRoarTime();
            searchingRoar.Play();
        }
    }

    Vector3 lastPosition = Vector3.zero;
    Vector3 moveLocation = Vector3.zero;
    float curRadius = 0f;

    void HuntPlayer()
    {
        // Rudolph should hunt down the player by "searching" for them. Rudolph should "smell" them based on how far they are, and should walk towards a random point close to the player (depending on how well Rudolph can smell them). If Rudolph gets close enough to the player, Rudolph should start chasing them. If the player gets outside of the smell range, a new random point should be chosen.
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (SeesPlayer() || HearsPlayer())
        {
            
            locatedRoar.Play();
            currentState = State.Chasing;
            moveLocation = player.transform.position;
            curRadius = 0f;
        }
        else if (Vector2.Distance(new(transform.position.x, transform.position.z), new(moveLocation.x, moveLocation.z)) < 3f || 
                 Vector3.Distance(moveLocation, player.transform.position) > curRadius || 
                 lastPosition == transform.position) // Pick a new point if Rudolph is stuck
        {
            float smell = Mathf.Pow(smellExponentialDecay, distance);

            curRadius = Math.Clamp(smell, minSmellRadius, maxSmellRadius);
            // Find a safe position for Rudolph to move to. Only try 3 times, otherwise game would crash.
            int tries = 0;
            NavMeshHit hit;
            do {
                Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * curRadius;
                randomPoint.y = 0f;
                var randomOffset = UnityEngine.Random.insideUnitSphere * randomMovePointRadiusOffset;
                moveLocation = player.transform.position + randomPoint + randomOffset;
                moveLocation = FindGround(moveLocation);
            } while (!NavMesh.SamplePosition(moveLocation, out hit, 1f, NavMesh.AllAreas) && tries++ < 3);
            moveLocation = hit.position;

        }
        agent.SetDestination(moveLocation);
        lastPosition = transform.position;
    }

    float hideTime = 0f;
    float walkTime = 0f;

    void ChasePlayer()
    {
        bool seesPlayer = SeesPlayer();
        bool hearsPlayer = HearsPlayer();
        // If Rudolph hears the player's footsteps, it can find them.
        if (hearsPlayer || seesPlayer) {
            hideTime = 0f;
            agent.SetDestination(player.transform.position);
        }
        // If rudolph can't see the player, it will slowly lose interest (while still chasing them down)
        if (!seesPlayer && hideTime < loseInterestTime)
        {
            hideTime += Time.deltaTime;
        }
        // If Rudolph hasn't seen the player for a given amount of time, Rudolph should go back to hunting
        else if (!seesPlayer && hideTime >= loseInterestTime && walkTime < walkAwayTime)
        {
            walkTime += Time.deltaTime;
            agent.SetDestination(transform.position);
        }
        else if (!seesPlayer && walkTime >= walkAwayTime)
        {
            currentState = State.Hunting;
            hideTime = 0f;
            walkTime = 0f;
            canIdle = true;
        }
        else {
            hideTime = 0f;
            walkTime = 0f;
            agent.SetDestination(player.transform.position);
        }
        
        if (Vector3.Distance(transform.position, player.transform.position) < attackDistance)
        {
            currentState = State.Attacking;
        }
    }

    public void LostPlayerRoar()
    {
        lostPlayerRoar.Play();
    }

    void AttackPlayer()
{
    player.GetComponent<Collider>().enabled = false;
    // Freeze the player in place
    player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

    // Look at Rudolph's face
    Vector3 rudolphFacePosition = transform.position + new Vector3(0, 2.3f, 0);
    player.transform.LookAt(rudolphFacePosition, Vector3.up);

    agent.SetDestination(transform.position);
    GameManager.Instance.EndGame();
    locatedRoar.pitch = 2f;
    locatedRoar.Play();
}

}
