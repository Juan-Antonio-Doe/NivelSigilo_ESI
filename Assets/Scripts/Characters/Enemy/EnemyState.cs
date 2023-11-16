using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState {

    public enum STATE {
        Idle,           // This is the paused state.
        Patrol,         // Enemy is patrolling through the waypoints.
        Look,           // Enemy is looking for the player. After an alert (camera or corpse detected, after a chase).
        Investigate,    // Enemy is investigating the last player position detected by a camera.
    }

    public enum STAGES {
        Enter,
        Update,
        Exit
    }

    public STATE currentState { get; set; }
    protected STAGES stage { get; set; }
    protected GameObject npc { get; set; }  // The current enemy gameobject.
    protected EnemyController enemy { get; set; }
    protected PlayerManager player { get; set; }
    protected NavMeshAgent agent { get; set; }
    protected EnemyState nextState { get; set; }

    public EnemyState(GameObject npc, EnemyController enemy, NavMeshAgent agent, PlayerManager player) {

        this.npc = npc;
        this.enemy = enemy;
        this.agent = agent;
        this.player = player;
        stage = STAGES.Enter;
    }

    public virtual void Enter() {
        stage = STAGES.Update;
    }

    public virtual void Update() {
        stage = STAGES.Update;
    }

    public virtual void Exit() {
        stage = STAGES.Exit;
    }

    /// <summary>
    /// This method is used to switch between the different methods that change the state.
    /// </summary>
    public EnemyState Process() {
        if (stage == STAGES.Enter) Enter();
        if (stage == STAGES.Update) Update();
        if (stage == STAGES.Exit) {
            Exit();
            return nextState; // It returns us the state that would touch next.
        }

        // This would return us to the same state we are in if none of the above conditions are met.
        return this;
    }

    public bool CanSeePlayer() {
        return enemy.enemies.PlayerDetected;
    }
}
