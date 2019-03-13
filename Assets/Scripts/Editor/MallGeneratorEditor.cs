using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MallGenerator))]
public class MallGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        MallGenerator mallGenerator = target as MallGenerator;

        if (GUILayout.Button("Clear Mall")) {
            mallGenerator.ClearMall();
        }
        if (GUILayout.Button("Generate Mall")) {
            mallGenerator.GenerateMall();
        }
        if (GUILayout.Button("TO DO")) {
            //mallGenerator.FindPathOnButtonPress();
            Debug.Log("Make listner on the NavigationManager");
        }

    }
}