using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    [field: Header("AutoAttach on Editor properties")]
    [field: SerializeField, FindObjectOfType, ReadOnlyField] public PlayerManager player { get; private set; }

    // Used for alert to others enemies.
    [field: SerializeField, ReadOnlyField] private bool _playerDetected { get; set; }
    public bool PlayerDetected { get { return _playerDetected; } set { _playerDetected = value; } }

    void Start() {
        
    }

    void Update() {
        
    }
}
