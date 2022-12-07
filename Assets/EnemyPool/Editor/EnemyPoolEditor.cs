using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(EnemyPool))]
public class EnemyPoolEditor : Editor
{
    private EnemyPool _enemyPool;
    private SerializedProperty _spawnedEnemies;

    private void OnEnable()
    {
        _enemyPool = (EnemyPool)target;
        _spawnedEnemies = serializedObject.FindProperty("_spawnedEnemies");

        if (_enemyPool.SpawnDatas == null || _enemyPool.SpawnDatas.Count == 0)
        {
            _enemyPool.InitializeSpawnData();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(_spawnedEnemies);

        int enemyTypesCount = System.Enum.GetNames(typeof(Enemies)).Length;

        for (int i = 0; i < enemyTypesCount; i++)
        {
            Enemies enemyType = (Enemies)(1 << i);
            if (_enemyPool.SpawnedEnemies.HasFlag(enemyType))
            {
                SpawnData spawnData = GetSpawnData(enemyType);
                if (spawnData != null)
                    DrawEnemtSettings(spawnData);
            }
        }

        GUILayout.BeginHorizontal();
        DrawResetButton();
        DrawSpawnButton();
        GUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_enemyPool);
            EditorSceneManager.MarkSceneDirty(_enemyPool.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawEnemtSettings(SpawnData spawnData)
    {
        GUILayout.BeginVertical(GUI.skin.window);
        GUILayout.Label(spawnData.EnemiesType.ToString());
        spawnData.IsRandomCount = EditorGUILayout.Toggle("Is Random Count", spawnData.IsRandomCount);
        if (spawnData.IsRandomCount)
            spawnData.Range = EditorGUILayout.Vector2IntField("Random Range", spawnData.Range);
        else
        {
            GUILayout.BeginHorizontal();
            spawnData.Count = EditorGUILayout.IntField("Count", spawnData.Count,GUILayout.ExpandWidth(false));
            spawnData.Count = EditorGUILayout.IntField("Count", spawnData.Count, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }

    private void DrawResetButton()
    {
        if (GUILayout.Button("Reset", GUILayout.Width(100)))
        {
            _enemyPool.InitializeSpawnData();
            Debug.Log(_spawnedEnemies.intValue);
            _spawnedEnemies.intValue = int.MaxValue;
        }
    }

    private void DrawSpawnButton()
    {
        if (GUILayout.Button("Spawn", GUILayout.Width(100)))
        {
            //_enemyPool.Spawn();
        }
    }

    private SpawnData GetSpawnData(Enemies enemyType)
    {
        for (int i = 0; i < _enemyPool.SpawnDatas.Count; i++)
        {
            if (_enemyPool.SpawnDatas[i].EnemiesType == enemyType)
                return _enemyPool.SpawnDatas[i];
        }

        return null;
    }
}
