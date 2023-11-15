using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handle the enemy's idle state.
/// </summary>

public class EnemyIdle : EnemyState {
    public EnemyIdle(GameObject npc, EnemyController enemy, NavMeshAgent agent) : base(npc, enemy, agent) {

        currentState = STATE.Idle;
    }

    public override void Enter() {
        if (enemy.Waypoints.Count > 0) {    // If the enemy has waypoints, switch to the patrol state.
            nextState = new EnemyPatrol(npc, enemy, agent/*, player*/);

            stage = STAGES.Exit;
            return;
        }

        base.Enter();
    }

    public override void Update() {
        base.Update();
    }
}