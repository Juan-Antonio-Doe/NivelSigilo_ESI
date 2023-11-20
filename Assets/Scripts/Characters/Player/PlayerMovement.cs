using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    [field: Header("Autoattach Inspector Properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerInput _playerInput { get; set; }
    //[field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody _rb { get; set; }
    //[field: SerializeField, GetComponent, ReadOnlyField] private CapsuleCollider _collider { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private NavMeshAgent _agent { get; set; }

    /*[field: Header("Movement Properties")]
    [field: SerializeField] private float _movementSpeed = 5f;*/

    private Vector2 _movementInput;

    /*private void FixedUpdate() {
        MoveDeprecated();
    }*/

    /*void MoveDeprecated() {
        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        movement = movement.normalized * _movementSpeed * Time.deltaTime;

        _rb.AddForce(movement, ForceMode.VelocityChange);
    }*/

    void Update() {
        //MoveAgent();

        if (_movementInput.sqrMagnitude <= 0) {
            return;
        }

        if (Mathf.Abs(_movementInput.y) > 0.01f) {
            MoveAgentAlternative();
        }
        else {
            RotateAgentAlternative();
        }
        
    }

    void MoveAgent() {
        Vector3 _scaledMovement = _agent.speed * Time.deltaTime * new Vector3(_movementInput.x, 0, _movementInput.y);

        _agent.transform.LookAt(_agent.transform.position + _scaledMovement, Vector3.up);

        // Normalize the vector to avoid faster diagonal movement
        if (_scaledMovement.sqrMagnitude > 1) {
            _scaledMovement.Normalize();
        }

        _agent.Move(_scaledMovement);
    }

    void MoveAgentAlternative() {
        Vector3 destination = transform.position + transform.right * _movementInput.x + transform.forward * _movementInput.y;
        _agent.destination = destination;
    }

    void RotateAgentAlternative() {
        _agent.destination = transform.position;
        transform.Rotate(0, _movementInput.x * _agent.angularSpeed * Time.deltaTime, 0);
    }

    /// <summary>
    /// Called when the player presses the movement keys.
    /// </summary>
    void OnMovement(InputValue value) {
        _movementInput = value.Get<Vector2>();
    }
}