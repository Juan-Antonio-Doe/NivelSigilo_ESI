using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement_2 : MonoBehaviour {

    [field: Header("Autoattach Inspector Properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody _rigidbody { get; set; }

    [field: Header("Movement Properties")]
    [field: SerializeField] private float _moveSpeed { get; set; } = 10.0f;
    [field: SerializeField] private float _turnSpeed { get; set; } = 100.0f;

    [field: Header("Camera Properties")]
    [field: SerializeField] private Transform _camera { get; set; }

    private Vector3 _moveInput { get; set; }
    private Vector2 _movementInput { get; set; }
    private float _turnInput { get; set; }

    void Start() {
        
    }

    void FixedUpdate() {
        Movement();
    }

    void Movement() {
        // Update movement input.
        _moveInput = new Vector3(_movementInput.x, 0, _movementInput.y) * _moveSpeed;

        // Update rotation input.
        _turnInput = Input.GetAxis("Mouse X") * _turnSpeed;

        // Apply rotation to movement input.
        _moveInput = _camera.InverseTransformDirection(_moveInput);

        // Apply movement.
        _rigidbody.AddForce(_moveInput);

        // Apply rotation.
        transform.Rotate(Vector3.up, _turnInput * Time.deltaTime);

        // Update camera.
        _camera.LookAt(transform);
    }

    /// <summary>
    /// Called when the player presses the movement keys.
    /// </summary>
    void OnMovement(InputValue value) {
        _movementInput = value.Get<Vector2>();
    }
}
