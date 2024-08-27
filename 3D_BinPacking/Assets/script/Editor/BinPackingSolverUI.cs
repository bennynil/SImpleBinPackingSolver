using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(BinPackingSolver))]
public class BinPackingSolverUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BinPackingSolver binps = (BinPackingSolver)target;
        if (GUILayout.Button("solve"))
        {
            //binps.solveTestingFunction();
        }
        if (GUILayout.Button("visual"))
        {
            binps.showBoxesCoroutine();
        }

    }
}
