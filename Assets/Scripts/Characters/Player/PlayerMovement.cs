using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

    [field: Header("Autoattach Inspector Properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private PlayerInput _playerInput { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody _rb { get; set; }
    [field: SerializeField, GetComponent, ReadOnlyField] private CapsuleCollider _collider { get; set; }

    [field: Header("Movement Properties")]
    [field: SerializeField] private float _movementSpeed = 5f;

    private Vector2 _movementInput;

    /*void OnEnable() {
        _playerInput.actions["Movement"].performed += OnMovement;
        _playerInput.actions["Movement"].canceled += OnMovement;
    }*/

    private void FixedUpdate() {
        Move();
    }

    void Move() {
        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        movement = movement.normalized * _movementSpeed * Time.deltaTime;

        _rb.AddForce(movement, ForceMode.VelocityChange);
    }

    void OnMovement(InputValue value) {
        _movementInput = value.Get<Vector2>();
    }

    /*void OnDisable() {
        _playerInput.actions["Movement"].performed -= OnMovement;
        _playerInput.actions["Movement"].canceled -= OnMovement;
    }*/
}