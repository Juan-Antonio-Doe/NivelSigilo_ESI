using System;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handle the enemy's idle state.
/// </summary>

public class EnemyPatrol : EnemyState {

    private int _lastWaypointIndex = 0;

    public EnemyPatrol(GameObject npc, EnemyController enemy, NavMeshAgent agent, PlayerManager player) : base(npc, enemy, agent, player) {

        currentState = STATE.Patrol;
        agent.speed = 4f;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Enter() {

        //Debug.Log($"Enemy {enemy.name} is in waypoint {enemy.CurrentWaypointIndex}.");
        GotoNextPoint();
        //Debug.Log($"Enemy {enemy.name} is going to waypoint {enemy.CurrentWaypointIndex}.");

        if (enemy.PatrolSpeedModifier > 0)
            agent.speed *= enemy.PatrolSpeedModifier;

        base.Enter();
    }

    public override void Update() {

        // If we've reached the destination
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance) {
            // If we are going to stop in each waypoint, we will change the state to idle.
            if (enemy.StopInEachWaypoint) {
                nextState = new EnemyIdle(npc, enemy, agent, player);

                stage = STAGES.Exit;
                return;
            }
            else {
                if (enemy.StopInEachWaypoint) {
                    ExitToIdle();
                    return;
                }
                else if (enemy.WaypointsToStop != null && enemy.WaypointsToStop.Length > 0) {
                    // When we reach at specific waypoint, we will change to idle state.
                    if (Array.Exists(enemy.WaypointsToStop, element => element == _lastWaypointIndex + 1)) {
                        ExitToIdle();
                        return;
                    }
                    else
                        GotoNextPoint();
                }
                else {
                    // When we reach at the last or first waypoint, we will change to idle state.
                    if (_lastWaypointIndex == enemy.Waypoints.Count - 1 || _lastWaypointIndex == 0) {
                        ExitToIdle();
                        return;
                    }
                    else
                        GotoNextPoint();
                }
            }
        }

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
            if ((enemy.TargetWaypointIndex + 1) > enemy.Waypoints.Count - 1) {
                enemy.InvertPatrol = true;
            }
            else if (enemy.TargetWaypointIndex - 1 < 0) {
                enemy.InvertPatrol = false;
            }
        }


        // Set the agent to go to the currently selected destination.
        agent.destination = enemy.Waypoints[enemy.TargetWaypointIndex].position;

        //Debug.Log($"Enemy {enemy.name} is in waypoint {enemy.TargetWaypointIndex}.");

        // Store the last waypoint index.
        _lastWaypointIndex = enemy.TargetWaypointIndex;

        /*
         * Choose the next point in the array as the destination,
         * cycling to the start if necessary.
         */
        if (!enemy.InvertPatrol)
            enemy.TargetWaypointIndex = (enemy.TargetWaypointIndex + 1) % enemy.Waypoints.Count;
        else
            // Invert the direction if we've reached the end of the path
            enemy.TargetWaypointIndex = (enemy.TargetWaypointIndex - 1) % enemy.Waypoints.Count;
    }

    void ExitToIdle() {
        nextState = new EnemyIdle(npc, enemy, agent, player);

        //Debug.Log($"Enemy {enemy.name} has reached to first or last waypoint. Changing to idle state.");
        //Debug.Log($"Target: {_lastWaypointIndex} and Count: {enemy.Waypoints.Count}");

        stage = STAGES.Exit;
    }
}
