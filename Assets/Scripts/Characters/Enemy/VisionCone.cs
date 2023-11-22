using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the vision cone and detections.
/// </summary>

[AddComponentMenu("Scripts/ESI/Characters/Vision Cone"), RequireComponent(typeof(SphereCollider))]
public class VisionCone : MonoBehaviour {

    [field: Header("AutoAttach on Editor properties")]
    [field: SerializeField, FindObjectOfType] public EnemyManager enemies { get; private set; }
    [field: SerializeField, GetComponent] private EnemyController enemyController { get; set; }
    [field: SerializeField, GetComponent] public SphereCollider sphereCollider { get; private set; }

    [field: Header("Other properties")]
    [field: Header("Cone settings")]
    [field: SerializeField, Tooltip("Radius of the cone.")] private float _viewRadius { get; set; } = 6f;
    public float ViewRadius { get { return _viewRadius; } set { _viewRadius = value; } }

    [field: Range(0, 360)]
    [field: SerializeField, Tooltip("Angle of the cone.")] private float _viewAngle { get; set; } = 100f;
    public float ViewAngle { get { return _viewAngle; } }

    [field: Header("Masks")]
    [field: SerializeField, Tooltip("Mask of the targets.")] public LayerMask targetMask { get; private set; }
    [field: SerializeField, Tooltip("Mask of the obstacles.")] public LayerMask obstacleMask { get; private set; }

    [field: Header("View Mesh settings")]
    [field: SerializeField, 
        Tooltip("Raycasts amount for generate the cone.")] private float _meshResolution { get; set; } = 0.07f;    
    [field: SerializeField, Tooltip("Times to iterate to search for the edge of an obstacle.")
        ] private int _edgeResolveIterations { get; set; } = 4;
    [field: SerializeField, Tooltip("Distance by which it will be considered if the second object against which the " +
        "cone hits is a different object or not. This value is used to adjust the cone not only to an object but also " +
        "to another that it collides with having a distance between said objects.")
        ] private float _edgeDistanceThreshold { get; set; } = 0.5f;
    [field: SerializeField] public MeshFilter _viewMeshFilter { get; private set; }
    private Mesh _viewMesh { get; set; }

    [field: Header("Others settings for mechanics")]
    [field: Header("Sleeping Guard")]
    [field: SerializeField] private float _minViewRadius { get; set; } = 1.5f;
    public float MinViewRadius { get { return _minViewRadius; } set { _minViewRadius = value; } }
    private float _maxViewRadius { get; set; } = 6f;
    public float MaxViewRadius { get { return _maxViewRadius; } }

    [field: Header("Debug")]
    [field: Header("Dead")]
    [field: SerializeField, ReadOnlyField] private bool _stopFoV;
    public bool StopFoV {
        get { return _stopFoV; }
        set {
            _stopFoV = value;
        }
    }
    
    private bool _playerDetected;
    public bool PlayerDetected { get { return _playerDetected; } }
    
    private bool _corpseDetected;
    public bool CorpseDetected { get { return _corpseDetected; } }

    private void OnValidate() {
        if (sphereCollider != null && sphereCollider.radius != _viewRadius && sphereCollider.isTrigger) {
            sphereCollider.radius = _viewRadius;
            sphereCollider.isTrigger = true;
        }
    }

    void Start() {
        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";
        _viewMeshFilter.mesh = _viewMesh;
        _viewMesh.MarkDynamic();

        _maxViewRadius = _viewRadius;
    }

    void LateUpdate() {
        if (StopFoV) {
            _viewMeshFilter.mesh = null;
            return;
        }

        DrawFieldOfView();
    }

    // Convert an angle to a direction.
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void DrawFieldOfView() {
        // Convert the number of rays will cast per degree to an integer.
        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float stepAngleSize = _viewAngle / stepCount;   // Calculate the angle between each ray.
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        // Loop through each ray.
        for (int i = 0; i <= stepCount; i++) {
            float angle = transform.eulerAngles.y - _viewAngle / 2 + stepAngleSize * i;
            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * viewRadius, Color.red);
            ViewCastInfo newViewCast = ViewCast(angle);

            // If the ray hits an obstacle, cast a ray from the edge of the obstacle.
            if (i > 0) {
                bool edgeDistanceThresholdExceeded = 
                    Mathf.Abs(oldViewCast.distance - newViewCast.distance) > _edgeDistanceThreshold;
                
                if (oldViewCast.hitting != newViewCast.hitting ||
                    (oldViewCast.hitting && newViewCast.hitting && edgeDistanceThresholdExceeded)) {

                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero) {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero) {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        // Draw the view mesh.
        
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero; // The first vertex is the player's position.

        for (int i = 0; i < vertexCount - 1; i++) {
            // InverseTransformPoint: Convert a point from world space to local space.
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]); // Fix the mesh's vertexs position.

            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
        _viewMesh.Optimize();
    }

    // Handle the raycast info.
    ViewCastInfo ViewCast(float globalAngle) {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, _viewRadius, obstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else {
            return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius, globalAngle);
        }
    }

    // Store the raycast information.
    private struct ViewCastInfo {
        public bool hitting;
        public Vector3 point;
        public float distance;  // The distance from the origin to the hit point.
        public float angle;

        public ViewCastInfo(bool _hitting, Vector3 _point, float _distance, float _angle) {
            hitting = _hitting;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    // Find the edge of the obstacle.
    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < _edgeResolveIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > _edgeDistanceThreshold;
            if (newViewCast.hitting == minViewCast.hitting && !edgeDistanceThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    // Store the points to use for find the edge of the obstacle.
    public struct EdgeInfo {
        public Vector3 pointA, pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

    /// <summary>
    /// Stop the field of view.
    /// </summary>
    public void StopView() {
        StopFoV = true;
        _viewMeshFilter.mesh = null;
        enabled = false;
    }

    private void OnTriggerStay(Collider other) {
        if (!enabled)
            return;

        if (other.GetType() != typeof(SphereCollider)) {    // Ignore all SphereColliders.
            if (other.gameObject != gameObject || !other.transform.IsChildOf(transform)) { // Ignore itself.
                if (!StopFoV) {

                    Vector3 direction = Vector3.zero;

                    if (other.CompareTag("Player"))
                        direction = enemies.player.transform.position - (transform.position + transform.up);
                    else
                        return;

                    float angle = Vector3.Angle(direction, transform.forward);

                    if (angle < _viewAngle / 2f) {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, direction.normalized, out hit, _viewRadius, targetMask)) { //obstacleMask / targetMask
                            CheckIfPlayerIsInSight(other, hit);
                        }
                        //Debug.DrawRay(transform.position, direction.normalized * _viewRadius, Color.red);
                        //Debug.Log($"Other = {other.name}");
                    }
                }

                if (enemyController.DetectPlayerBySound) {
                    if (other.CompareTag("Player")) {
                        if (Vector3.Distance(transform.position, enemies.player.transform.position) <= enemyController.SoundDetectionRadius) {
                            PlayerDetection(true);
                        }
                    }
                }

                if (enemyController.DetectPlayerByProximity) {
                    if (other.CompareTag("Player")) {
                        if (Vector3.Distance(transform.position, enemies.player.transform.position) <= enemyController.ProximityDetectionRadius) {
                            PlayerDetection(true);
                        }
                    }
                }
            }
        }
    }

    private void CheckIfPlayerIsInSight(Collider other, RaycastHit hit) {
        if (hit.collider.CompareTag("Player")) {
            Debug.Log("Player Detected");
            PlayerDetection(true);
        }
        else if (_playerDetected) {
            PlayerDetection(false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (!enabled)
            return;

        if (other.CompareTag("Player"))
            PlayerDetection(false);
    }

    private void PlayerDetection(bool detected) {
        _playerDetected = detected;
        enemies.PlayerDetected = detected;
    }
}
