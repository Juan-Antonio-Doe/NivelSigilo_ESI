using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handle the enemy's idle state.
/// </summary>

public class EnemyPatrol : EnemyState {
    public EnemyPatrol(GameObject npc, EnemyController enemy, NavMeshAgent agent) : base(npc, enemy, agent) {

        currentState = STATE.Patrol;
        agent.speed = 4f;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Enter() {
        GotoNextPoint();

        if (enemy.PatrolSpeedModifier > 0)
            agent.speed *= enemy.PatrolSpeedModifier;

        base.Enter();
    }

    public override void Update() {

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance) // If we've reached the destination
            GotoNextPoint();

        base.Update();
    }

    public override void Exit() {
        agent.ResetPath();
        base.Exit();
    }

    void GotoNextPoint() {
        // Returns if no points have been set up
        if (enemy.Waypoints.Count == 0)
            return;

        // Set the direction of the patrol.
        if (enemy.PatrolType == EnemyController.PatrolTypeEnum.PingPong) {
            if ((enemy.CurrentWaypointIndex + 1) > enemy.Waypoints.Count - 1) {
                enemy.InvertPatrol = true;
            }
            else if (enemy.CurrentWaypointIndex - 1 < 0) {
                enemy.InvertPatrol = false;
            }
        }

        //Debug.Log(currentWaypointIndex);

        // Set the agent to go to the currently selected destination.
        agent.destination = enemy.Waypoints[enemy.CurrentWaypointIndex].position;

        /*
         * Choose the next point in the array as the destination,
         * cycling to the start if necessary.
         */
        if (!enemy.InvertPatrol)
            enemy.CurrentWaypointIndex = (enemy.CurrentWaypointIndex + 1) % enemy.Waypoints.Count;
        else
            // Invert the direction if we've reached the end of the path
            enemy.CurrentWaypointIndex = (enemy.CurrentWaypointIndex - 1) % enemy.Waypoints.Count;
    }
}