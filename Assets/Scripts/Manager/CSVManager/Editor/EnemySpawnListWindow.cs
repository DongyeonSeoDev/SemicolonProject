#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Enemy;
using System.Collections.Generic;

public class EnemySpawnListWindow : EditorWindow
{
    private EnemySpawn enemySpawn;
    private SerializedObject serializedObject;
    private SerializedProperty serializedProperty;
    private Vector2 scrollPosition = Vector2.zero;
    private string findText = "";
    private string lastFindText = "";
    private int selectSortIndex = 0;
    private int lastSelectSortIndex = 0;

    private List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>();

    private string[] sortName = new string[2] { "StageSort", "EnemySort" };

    private void Sort(int sortIndex)
    {
        switch (selectSortIndex)
        {
            case 0:
                enemySpawn.enemySpawnData.Sort((x, y) => 
                {
                    int value = x.stageId.CompareTo(y.stageId);

                    return value == 0 ? x.enemyId.CompareTo(y.enemyId) : value;
                });
                break;
            case 1:
                enemySpawn.enemySpawnData.Sort((x, y) =>
                {
                    int value = x.enemyId.CompareTo(y.enemyId);

                    return value == 0 ? x.stageId.CompareTo(y.stageId) : value;
                });

                break;
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    private void FindText(string text)
    {
        enemySpawn.enemySpawnData.Clear();

        if (string.IsNullOrEmpty(text))
        {
            for (int i = 0; i < enemySpawnDataList.Count; i++)
            {
                enemySpawn.enemySpawnData.Add(enemySpawnDataList[i]);
            }
        }
        else
        {
            for (int i = 0; i < enemySpawnDataList.Count; i++)
            {
                if (enemySpawnDataList[i].name.Contains(text))
                {
                    enemySpawn.enemySpawnData.Add(enemySpawnDataList[i]);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }

    private void OnEnable()
    {
        enemySpawn = FindObjectOfType<EnemySpawn>();
        serializedObject = new SerializedObject(enemySpawn);
        serializedProperty = serializedObject.FindProperty("enemySpawnData");

        CSVEnemySpawn.Instance.GetData("");
        enemySpawnDataList.Clear();

        CSVEnemySpawn.Instance.enemySpawnDatas.Values.ForEach(datas =>
        {
            datas.ForEach(data =>
            {
                enemySpawnDataList.Add(data);
            });
        });

        FindText(findText);
        Sort(selectSortIndex);
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(5);

        selectSortIndex = EditorGUILayout.Popup("Sort", selectSortIndex, sortName);
       
        if (lastSelectSortIndex != selectSortIndex)
        {
            lastSelectSortIndex = selectSortIndex;

            Sort(selectSortIndex);
        }

        EditorGUILayout.Space(2);

        findText = EditorGUILayout.TextField("Find Text", findText);

        if (findText != lastFindText)
        {
            lastFindText = findText;

            FindText(findText);
        }

        EditorGUILayout.Space(5);

        EditorGUIUtility.wideMode = true;
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedProperty, new GUIContent("EnemySpawnData"));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndScrollView();
        EditorGUIUtility.wideMode = false;
    }
}
#endif