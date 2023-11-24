using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : MonoBehaviour {

    [field: Header("Item properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private Rigidbody rb { get; set; }
    public Rigidbody Rb { get { return rb; } }
    [field: SerializeField] private float timeBefroDestoy { get; set; } = 7f;

    /*private float throwForce { get; set; } = 1f;
    public float ThrowForce { get { return throwForce; } set { throwForce = value; } }

    private Transform throwPoint { get; set; }
    public Transform ThrowPoint { get { return throwPoint; } set { throwPoint = value; } }*/
	
    IEnumerator Start() {
        yield return new WaitForSeconds(timeBefroDestoy);
        Destroy(gameObject);
    }
}