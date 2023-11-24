using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowItem : MonoBehaviour {

    [field: Header("Throw properties")]
    [field: SerializeField] private ThrowableItem itemToThrowPrefab { get; set; }
    [field: SerializeField] private Transform throwPoint { get; set; }
    [field: SerializeField] private float throwForce { get; set; } = 1f;

    void OnThrow() {
        ThrowableItem itemToThrow = Instantiate(itemToThrowPrefab, throwPoint.position, itemToThrowPrefab.transform.rotation);
        itemToThrow.Rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
    }
	
}