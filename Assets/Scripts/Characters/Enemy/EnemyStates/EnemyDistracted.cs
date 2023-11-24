using UnityEngine;
using UnityEngine.AI;

public class EnemyDistracted : EnemyState {

    private float _currentDistractedTime;

    public EnemyDistracted(GameObject npc, EnemyController enemy, NavMeshAgent agent, PlayerMovement_2 player) 
        : base(npc, enemy, agent, player) {

        currentState = STATE.Distracted;
        agent.speed = 4f;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Enter() {
        _currentDistractedTime = enemy.DistractedTime;

        agent.SetDestination(enemy.DistractedPosition);

        base.Enter();
    }

    public override void Update() {

        // If the drone raises the alert or the player is detected, switch to alert state.
        if (OnAlert() || IndividualPlayerDectection()) {
            nextState = new EnemyAlert(npc, enemy, agent, player);

            stage = STAGES.Exit;
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance) {
            // Start counting down the distracted time. When it reaches 0, switch to patrol state.
            if (_currentDistractedTime <= 0) {
                nextState = new EnemyPatrol(npc, enemy, agent, player);

                stage = STAGES.Exit;
                return;
            }
            else
                _currentDistractedTime -= Time.deltaTime;
        }

        base.Update();
    }

    public override void Exit() {
        agent.ResetPath();
        enemy.IsDistracted = false;
        base.Exit();
    }
}