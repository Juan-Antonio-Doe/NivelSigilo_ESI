using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement_2 : MonoBehaviour {

    [field: Header("- Autoattach propierties -")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }

    [field: Header("Movement settings")]
    [field: SerializeField] private float moveSpeed { get; set; } = 6f;
    [field: SerializeField] private float movementMultiplier { get; set; } = 10f;
    [field: SerializeField, ReadOnlyField] private bool isSneaking { get; set; }
    public bool IsSneaking { get => isSneaking; }

    [field: Header("Camara settings")]
    [field: Range(1, 10)]
    [field: SerializeField] private float mouseSensitivity { get; set; } = 3f;
    [field: SerializeField] private Transform followTransform { get; set; }

    // Opciones para invertir el giro de la cmara.
    [field: SerializeField] private bool invertX;
    [field: SerializeField] private bool invertY;

    private float horizontalMovement { get; set; }
    private float verticalMovement { get; set; }

    private float rbDrag { get; set; } = 6f;

    private Vector3 moveDirection { get; set; }
    private Vector2 mouseInput;

    void Start() {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
    }

    void Update() {
        ControlDrag();
    }

    void FixedUpdate() {
        MovePlayer();
    }

    private void LateUpdate() {
        Rotation();
    }

    /// <summary>
    /// Called when the player presses the movement keys.
    /// </summary>
    void OnMovement(InputValue value) {
        Vector2 inputVector = value.Get<Vector2>();
        horizontalMovement = inputVector.x;
        verticalMovement = inputVector.y;
    }

    void OnCameraMovement(InputValue value) {
        mouseInput = value.Get<Vector2>();
    }

    void OnSneakMovement(InputValue value) {
        isSneaking = value.isPressed;
    }

    void MovePlayer() {

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

        float currentMoveSpeed = moveSpeed;

        if (isSneaking)
            currentMoveSpeed /= 2;

        rb.AddForce(moveDirection.normalized * currentMoveSpeed * movementMultiplier, ForceMode.Acceleration);
    }

    void Rotation() {
        // Aqui se modifica el movimiento sobre los ejes segn los gustos del jugador.
        if (invertX) {
            mouseInput.x = -mouseInput.x;
        }
        if (invertY) {
            mouseInput.y = -mouseInput.y;
        }

        // Player rotation
        //transform.rotation *= Quaternion.AngleAxis(mouseInput.x * mouseSensitivity, Vector3.up);

        float halfMouseSensitivity = mouseSensitivity / 2;

        // Rotamos al personaje según gira el ratón.
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x * halfMouseSensitivity,
        transform.rotation.eulerAngles.z);

        // Camera vertical rotation
        followTransform.rotation *= Quaternion.AngleAxis(mouseInput.y * halfMouseSensitivity, Vector3.right);

        Vector3 angles = followTransform.localEulerAngles;
        angles.z = 0;

        float angle = followTransform.localEulerAngles.x;

        // Clamp the Up/Down rotation
        if (angle > 180 && angle < 340) {
            angles.x = 340;
        } else if (angle < 180 && angle > 40) {
            angles.x = 40;
        }

        // Set the player's rotation based on the look transform
        //transform.rotation *= Quaternion.Euler(0, followTransform.eulerAngles.y, 0);
        // Reset the y rotation of the look transform
        followTransform.localEulerAngles = new Vector3(angles.x, 0, 0);
    }

    void ControlDrag() {
        rb.drag = rbDrag;
    }
}
