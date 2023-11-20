using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour {

    [field: SerializeField] private Transform _playerTransform { get; set; }
    private Vector3 _cameraOffset { get; set; }
    [field: SerializeField] private float _rotationSpeed { get; set; } = 1;

    //[field: SerializeField] private Transform _target { get; set; }


    private float mouseX { get; set; }
    private float mouseY { get; set; }

    [field: Range(0.01f, 1.0f)]
    [field: SerializeField] private float SmoothFactor { get; set; } = 0.5f;

    void Start() {
        _cameraOffset = transform.position - _playerTransform.position;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        CamControl();
        Vector3 newPos = _playerTransform.position + _cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, SmoothFactor);
    }

    void CamControl() {
        mouseX += Input.GetAxis("Mouse X") * _rotationSpeed;
        mouseY += Input.GetAxis("Mouse Y") * _rotationSpeed * -1;
        mouseY = Mathf.Clamp(mouseY, -35, 60);
        transform.LookAt(_playerTransform);
        _playerTransform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        //_playerTransform.rotation = Quaternion.Euler(0, mouseX, 0);
    }
}
