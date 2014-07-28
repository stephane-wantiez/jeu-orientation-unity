using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardGenerator))]
public class BoardGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BoardGenerator boardGen = target as BoardGenerator;

        if (GUILayout.Button("Generate board"))
        {
            boardGen.generateBoardFromEditor();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Reset board"))
        {
            boardGen.resetBoard();
            EditorUtility.SetDirty(target);
        }
    }
}
