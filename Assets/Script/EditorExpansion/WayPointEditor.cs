using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WayPointManager))]
public class WayPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WayPointManager manager = (WayPointManager)target;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("WayPointê∂ê¨",EditorStyles.boldLabel);
        if (GUILayout.Button("WayPointê∂ê¨"))
        {
            manager.GenerateWayPoints();
            Debug.Log("WayPointê∂ê¨äÆóπ");
        }
    }
}
