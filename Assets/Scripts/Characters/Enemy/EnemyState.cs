using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState {

    public enum STATE {
        Idle,           // This is the paused state.
        Patrol,         // Enemy is patrolling through the waypoints.
        Alert,          // Enemy is looking for the player.
        Distracted,     // Enemy is distracted by a sound.
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
    protected PlayerMovement_2 player { get; set; }
    protected NavMeshAgent agent { get; set; }
    protected EnemyState nextState { get; set; }

    public EnemyState(GameObject npc, EnemyController enemy, NavMeshAgent agent, PlayerMovement_2 player) {

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

    public bool PlayerDetected() {
        return enemy.enemies.PlayerDetected;
    }

    public bool OnAlert() {
        return enemy.enemies.DroneAlert;
    }
    
    public bool IndividualPlayerDectection() {
        return enemy.VisionCone.PlayerDetected;
    }
}
