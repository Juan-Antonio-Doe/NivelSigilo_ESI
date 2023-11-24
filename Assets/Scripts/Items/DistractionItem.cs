using Nrjwolf.Tools.AttachAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistractionItem : MonoBehaviour {

    [field: Header("Distraction properties")]
    [field: SerializeField, GetComponent, ReadOnlyField] private AudioSource soundDistraction { get; set; }
    [field: SerializeField] private float distractionRadius { get; set; } = 5f;
    [field: SerializeField] private float timeBefroSound { get; set; } = 2f;
    [field: SerializeField] private LayerMask targetMask { get; set; }

    private List<EnemyController> enemies = new List<EnemyController>();
	
    IEnumerator Start() {
        soundDistraction.maxDistance = distractionRadius * 2;
        yield return new WaitForSeconds(timeBefroSound);
        soundDistraction.Play();
    }

    void Update() {
        if (soundDistraction.isPlaying) {
            Collider[] enemies = Physics.OverlapSphere(transform.position, distractionRadius, targetMask);
            foreach (Collider enemy in enemies) {
                if (enemy.CompareTag("EnemyGuard")) {
                    // Add enemy to list if not already in
                    EnemyController enemyController = enemy.GetComponent<EnemyController>();

                    if (!this.enemies.Contains(enemyController)) {
                        this.enemies.Add(enemyController);
                    }

                    // If enemy is not already distracted, distract it
                    if (!enemyController.IsDistracted) {
                        enemyController.IsDistracted = true;
                        enemyController.DistractedPosition = transform.position;
                    }
                }
            }
        }
    }

    private void OnDestroy() {
        soundDistraction.Stop();
    }
}