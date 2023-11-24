using UnityEngine;
using UnityEngine.AI;

public class EnemyAlert : EnemyState {


    public EnemyAlert(GameObject npc, EnemyController enemy, NavMeshAgent agent, PlayerMovement_2 player) 
        : base(npc, enemy, agent, player) {

        currentState = STATE.Alert;
        agent.speed = 4f;
        agent.isStopped = false;
        agent.autoBraking = false;
    }

    public override void Enter() {
        agent.speed *= 1.25f;
        base.Enter();
    }

    public override void Update() {

        agent.destination = player.transform.position;

        base.Update();
    }
}