using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    [field: Header("AutoAttach on Editor properties")]
    [field: SerializeField, GetComponent] private NavMeshAgent _agent { get; set; }
    [field: SerializeField, FindObjectOfType] public EnemyManager enemies { get; private set; }

    [field: SerializeField, GetComponent] private CapsuleCollider _capsuleCollider { get; set; }
    public CapsuleCollider CapsuleCollider { get { return _capsuleCollider; } }


    [field: Header("Other Editor properties")]
    [field: SerializeField, Tooltip("Parent of the empty objects to use as waypoints.")
        ]
    private Transform waypointsParent { get; set; }
    [field: SerializeField, ReadOnlyField] private List<Transform> waypoints = new List<Transform>();   // Normal patrol waypoints.
    public List<Transform> Waypoints { get { return waypoints; } }

    private void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage/* && prefabConnected*/) {
            // Variables that will only be checked when they are in a scene
            if (!Application.isPlaying)
                // Assing the waypoints on the inspector.
                if (waypointsParent != null && (waypoints.Count == 0 || waypoints.Count != waypointsParent.childCount)) {

                    waypoints.Clear();
                    foreach (Transform child in waypointsParent) {
                        waypoints.Add(child);
                    }
                }
                else if (waypointsParent == null && waypoints.Count > 0) {
                    waypoints.Clear();
                }
        }
#endif
    }

    [field: Header("Other properties")]

    [field: SerializeField] private float patrolSpeedModifier = 0f;
    public float PatrolSpeedModifier { get { return patrolSpeedModifier; } }
    [field: SerializeField] private bool stopInEachWaypoint;
    public bool StopInEachWaypoint { get { return stopInEachWaypoint; } }


    private int _currentWaypointIndex = 0;
    public int TargetWaypointIndex { get { return _currentWaypointIndex; } set { _currentWaypointIndex = value; } }
    private bool _invertPatrol;  // If the enemy is going to the next waypoint or to the previous one.
    public bool InvertPatrol { get { return _invertPatrol; } set { _invertPatrol = value; } }

    public enum PatrolTypeEnum { Cyclic, PingPong }
    [field: SerializeField, Tooltip("Type of route that the enemy will take." +
        "\n� Cyclic: The enemy will return to the first waypoint when he reaches the last one." +
        "\n� Ping Pong: The enemy will do the reverse path when it reaches the last waypoint.")
        ]
    private PatrolTypeEnum _patrolType { get; set; } = PatrolTypeEnum.PingPong;
    public PatrolTypeEnum PatrolType { get { return _patrolType; } }

    private List<Transform> allWaypoints = new List<Transform>();   // All the waypoints of the scene.
    public List<Transform> AllWaypoints { get { return allWaypoints; } }
    private int currentLookingWaypoint = 0; // The waypoint that the enemy is looking at.
    public int CurrentLookingWaypoint { get { return currentLookingWaypoint; } set { currentLookingWaypoint = value; } }

    private EnemyState currentState { get; set; }

    void Start() {

        _agent.autoBraking = false;  // Disable the speed reduction when the agent is close to the destination.

        currentState = new EnemyIdle(gameObject, this, _agent, enemies.player);
    }

    void Update() {
        currentState = currentState.Process();
    }
}