using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDroneController : MonoBehaviour {

    [field: Header("Autoattach properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private NavMeshAgent agent { get; set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private EnemyManager enemyManager { get; set; }

    [field: Header("Properties")]
    [field: SerializeField] private float patrolRadius { get; set; } = 50f;
    [field: SerializeField] private float minDistance { get; set; } = 10f;

    [field: SerializeField] private bool playerInSight { get; set; }
    public bool PlayerInSight { get => playerInSight; }

    void Update() {
        // If we've reached the destination
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance) {
            // Search for a random point in the navmesh
            GoToNextPoint();
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
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerInSight = false;
        }
    }
}
