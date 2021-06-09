using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameSettings : EditorWindow
{
    Vector2Int _boardSize = new Vector2Int(10, 18);
    int _targetScore = 85;

    float _gameTickDelay = 0.15f;
    float _fixedUpdateDelay = 0.05f;

    [MenuItem("Game/Game Settings")]
    public static void ShowWindow()
    {
        GetWindow(typeof(GameSettings));
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Board Settings", EditorStyles.boldLabel);

        _boardSize = 
            EditorGUILayout.Vector2IntField("Game Board Size", _boardSize);
        _targetScore = 
            EditorGUILayout.IntField("Target Score", _targetScore);

        GUILayout.Space(15);

        _gameTickDelay = 
            EditorGUILayout.FloatField("Game Tick Delay", _gameTickDelay);
        _fixedUpdateDelay = 
            EditorGUILayout.FloatField("Fixed Update Delay", _fixedUpdateDelay);

        GUILayout.Space(15);

        if (GUILayout.Button("Save Settings"))
            SaveToEditorPrefs();
    }

    private void SaveToEditorPrefs()
    {
        EditorPrefs.SetInt("BoardSizeX", _boardSize.x);
        EditorPrefs.SetInt("BoardSizeY", _boardSize.y);

        EditorPrefs.SetInt("TargetScore", _targetScore);
        EditorPrefs.SetFloat("GameTickDelay", _gameTickDelay);
        EditorPrefs.SetFloat("FixedUpdateDelay", _fixedUpdateDelay);
    }
}
