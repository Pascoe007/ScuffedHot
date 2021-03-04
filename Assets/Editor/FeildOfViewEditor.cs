using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FeildOfViewEditor : Editor
{
   void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleWeapons in fow.visibleWeapons)
        {
            Debug.Log("test");
            Handles.DrawLine(fow.transform.position, visibleWeapons.position);
            
        }
        Handles.color = Color.cyan;
        foreach (Transform Player in fow.Player)
        {
            Debug.Log("Test2");
            Handles.DrawLine(fow.transform.position, Player.position);
            
        }

    }
}
