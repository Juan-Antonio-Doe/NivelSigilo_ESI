using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handle the enemy's idle state.
/// </summary>

public class EnemyIdle : EnemyState {

    private float _currentIdleTime = 3f;
    private float _maxIdleTime = 3f;

    public EnemyIdle(GameObject npc, EnemyController enemy, NavMeshAgent agent, PlayerMovement_2 player) : base(npc, enemy, agent, player) {

        currentState = STATE.Idle;
    }

    public override void Update() {

        // If the drone raises the alert or the player is detected, switch to alert state.
        if (OnAlert() || IndividualPlayerDectection()) {
            nextState = new EnemyAlert(npc, enemy, agent, player);

            stage = STAGES.Exit;
            return;
        }

        // If the enemy is distracted, switch to distracted state.
        if (enemy.IsDistracted) {
            nextState = new EnemyDistracted(npc, enemy, agent, player);

            stage = STAGES.Exit;
            return;
        }

        // When idleTime is over, switch to patrol state.
        if (_currentIdleTime <= 0 && enemy.Waypoints.Count > 0) {
            nextState = new EnemyPatrol(npc, enemy, agent, player);

            //Debug.Log($"Enemy {enemy.name} has finished the idle time. Changing to patrol state.");

            stage = STAGES.Exit;
            return;
        }
        else if (_currentIdleTime > 0)
            _currentIdleTime -= Time.deltaTime;

        base.Update();
    }

    public override void Exit() {
        _currentIdleTime = _maxIdleTime;
        base.Exit();
    }
}