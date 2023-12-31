using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    [field: Header("AutoAttach on Editor properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public PlayerMovement_2 player { get; private set; }
    [field: SerializeField, FindObjectOfType, ReadOnlyField] private LevelManager levelManager { get; set; }

    [field: Header("Alert feedback")]
    [field: SerializeField] private Light directionalLight { get; set; }
    [field: SerializeField] private Color alertColor { get; set; } = Color.red;

    // Used for alert to others enemies.
    [field: Header("Debug")]
    [field: SerializeField, ReadOnlyField] private bool _droneAlert { get; set; }
    public bool DroneAlert {
        get => _droneAlert; 
        
        set {
            _droneAlert = value;

            if (value)
                directionalLight.color = alertColor;
        }
    }
    [field: SerializeField, ReadOnlyField] private bool _playerDetected { get; set; }
    public bool PlayerDetected { get { return _playerDetected; } 
        set { 
            _playerDetected = value; 

            if (value)
                levelManager.EndGame();
        } 
    }

    // This is used to store the Waypoint objects in the scene to create a list with just the Transform component.
    [field: SerializeField, ReadOnlyField] public List<Transform> allWaypoints { get; private set; } = new List<Transform>();

    private void OnValidate() {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        bool isValidPrefabStage = prefabStage != null && prefabStage.stageHandle.IsValid();
        bool prefabConnected = PrefabUtility.GetPrefabInstanceStatus(this.gameObject) == PrefabInstanceStatus.Connected;
        if (!isValidPrefabStage /*&& prefabConnected*/) {
            ValidateAssings();
        }
#endif
    }

    private void ValidateAssings() {
        // Get all the waypoints in the scene in a simplified way.
        if (allWaypoints.Count == 0)
            allWaypoints = GameObject.FindGameObjectsWithTag("enemyWaypoint").Select(x => x.transform).ToList();

        if (directionalLight == null)
            directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
    }
}
