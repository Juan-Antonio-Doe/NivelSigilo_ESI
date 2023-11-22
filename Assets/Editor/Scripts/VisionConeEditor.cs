using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisionCone))]
public class VisionConeEditor : Editor {

	private void OnSceneGUI() {
        VisionCone fov = (VisionCone)target;
        Handles.color = Color.white;
        // Draw a circle of view.
        //Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.ViewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.ViewAngle / 2, false);

        // Draw a cone of view.
        Handles.DrawWireArc(fov.transform.position, Vector3.up, viewAngleA, fov.ViewAngle, fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);

        Handles.color = Color.red;
        /*foreach (Transform visibleTarget in fov.visibleTargets) {
            Handles.DrawLine(fov.transform.position, visibleTarget.position);
        }*/
    }
}
