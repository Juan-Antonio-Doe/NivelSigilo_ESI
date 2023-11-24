using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDroneController : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private NavMeshAgent agent { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemyManager enemyManager { get; set; }

    [field: Header("Properties")]
    [field: SerializeField] private float patrolRadius { get; set; } = 50f;
    [field: SerializeField] private float minDistance { get; set; } = 10f;
    [field: SerializeField] private float timeBeforeAlert { get; set; } = 3f;
    [field: SerializeField] private float timePursuinLostPlayer { get; set; } = 5f;

    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private bool playerInSight { get; set; }
    public bool PlayerInSight { get => playerInSight; }
    [field: SerializeField, ReadOnlyField] private bool isTimeBeforeAlertRunning { get; set; }
    [field: SerializeField, ReadOnlyField] private bool isLostPlayerTimerRunning { get; set; }
    [field: SerializeField, ReadOnlyField] private bool isPursuingPlayer { get; set; }

    // Timers
    private float alertTimer;
    private float lostPlayerTimer;

    private bool isCoroutineRunning;

    void Update() {

        if (!playerInSight && !isPursuingPlayer && !enemyManager.DroneAlert) {
            // If we've reached the destination
            if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance) {
                // Search for a random point in the navmesh
                GoToNextPoint();
            }
        }
        else {             // If the player is in sight or alert is active, pursue him
            agent.destination = enemyManager.player.transform.position;
        }
    }

    void GoToNextPoint() {
        // Get a random point on the navmesh
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        Vector3 finalPosition = hit.position;

        // Check if the distance to the new destination is greater than the minimum distance
        if (Vector3.Distance(transform.position, finalPosition) > minDistance) {
            agent.destination = finalPosition;
        }
        else {
            GoToNextPoint();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            playerInSight = true;
            if (!isCoroutineRunning) {
                StartCoroutine(CountdownLogic());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInSight = false;
        }
    }

    /* 
     * Countdown logic:
     * 
     * 1. While the player is in sight, countdown alert timer.
     * 2. If the player is not in sight, start lost player timer and pause alert timer.
     * 3. If the player is not in sight, and lost player timer is over, reset alert timer and lost player timer. Then, go back to patrolling.
     * 4. If the player is in sight again after losing him, reset lost player timer and continue alert timer.
     * 5. If the alert timer is over, alert the enemy manager.
     */
    IEnumerator CountdownLogic() {
        isCoroutineRunning = true;
        isPursuingPlayer = true;

        while (isPursuingPlayer) {
            if (playerInSight) {
                if (isLostPlayerTimerRunning) {
                    isLostPlayerTimerRunning = false;
                    lostPlayerTimer = 0f;
                }

                if (!isTimeBeforeAlertRunning)
                    isTimeBeforeAlertRunning = true;

                if (!enemyManager.DroneAlert)
                    alertTimer += Time.deltaTime;

                if (alertTimer >= timeBeforeAlert) {
                    // Alert the enemy manager
                    enemyManager.DroneAlert = true;
                    ResetLogic();
                }
            }
            else {
                if (isTimeBeforeAlertRunning)
                    isTimeBeforeAlertRunning = false;

                if (!isLostPlayerTimerRunning)
                    isLostPlayerTimerRunning = true;

                lostPlayerTimer += Time.deltaTime;

                if (lostPlayerTimer >= timePursuinLostPlayer) {
                    // Go back to patrolling
                    lostPlayerTimer = 0f;
                    alertTimer = 0f;
                    isPursuingPlayer = false;
                }
            }
            yield return null; // If we don't yield here, Unity will freeze.
        }

        ResetLogic();
    }

    void ResetLogic() {
        isTimeBeforeAlertRunning = false;
        isLostPlayerTimerRunning = false;
        alertTimer = 0f;
        lostPlayerTimer = 0f;
        isCoroutineRunning = false;
        isPursuingPlayer = false;
        StopCoroutine(CountdownLogic());
    }
}
