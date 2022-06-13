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
    private bool isReadonly = true;
    private bool isCreateBackUpFile = true;
    private bool isPlaying = false;
    private bool isSave = true;

    private List<EnemySpawnData> enemySpawnDataList = new List<EnemySpawnData>();
    private List<EnemySpawnData> revertList = new List<EnemySpawnData>();
    private List<EnemySpawnData> backUpList = new List<EnemySpawnData>();

    private string[] sortName = new string[2] { "StageSort", "EnemySort" };

    private void Refresh()
    {
        Repaint();
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
        SaveData();
        NameCheck();
    }

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

                enemySpawnDataList.Sort((x, y) =>
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

                enemySpawnDataList.Sort((x, y) =>
                {
                    int value = x.enemyId.CompareTo(y.enemyId);

                    return value == 0 ? x.stageId.CompareTo(y.stageId) : value;
                });
                break;
        }
    }

    private void FindText(string text)
    {
        enemySpawn.enemySpawnData.Clear();
        backUpList.Clear();

        if (string.IsNullOrEmpty(text))
        {
            for (int i = 0; i < enemySpawnDataList.Count; i++)
            {
                enemySpawn.enemySpawnData.Add(enemySpawnDataList[i]);
                backUpList.Add(enemySpawnDataList[i]);
            }
        }
        else
        {
            for (int i = 0; i < enemySpawnDataList.Count; i++)
            {
                if (enemySpawnDataList[i].name.Contains(text))
                {
                    enemySpawn.enemySpawnData.Add(enemySpawnDataList[i]);
                    backUpList.Add(enemySpawnDataList[i]);
                }
            }
        }
    }

    private void Save(bool isBackUp)
    {
        isSave = true;

        SaveList();
        SaveCSV(isBackUp);
        FindText(findText);
        Sort(selectSortIndex);
    }

    private void SaveCSV(bool isBackUp)
    {
        CSVEnemySpawn enemySpawn = new CSVEnemySpawn();
        enemySpawn.enemySpawnData = enemySpawnDataList;
        enemySpawn.SetData(isBackUp);
    }

    private void Revert()
    {
        if (RevertEvent())
        {
            isSave = true;

            RevertList();
            FindText(findText);
            Sort(selectSortIndex);
        }
    }

    private void SaveData()
    {
        for (int i = 0; i < enemySpawn.enemySpawnData.Count; i++)
        {
            int index = enemySpawnDataList.FindIndex(x => x.name == enemySpawn.enemySpawnData[i].name);

            if (index > -1)
            {
                EnemySpawnData data = enemySpawn.enemySpawnData[i];

                data.name = data.enemyId + " " + data.stageId + " " + data.position;

                if (enemySpawnDataList.FindIndex(x => x.name == data.name) <= -1)
                {
                    isSave = false;
                    enemySpawnDataList[index] = data;
                }

                enemySpawn.enemySpawnData[i] = data;
            }
        }
    }

    private void NameCheck()
    {
        List<int> indexCheck = new List<int>();
        int index = 0;
        bool isCheck = false;

        if (backUpList.Count != 0 && enemySpawn.enemySpawnData.Count != backUpList.Count)
        {
            isCheck = true;
        }

        for (int i = 0; i < enemySpawn.enemySpawnData.Count; i++)
        {
            index = enemySpawnDataList.FindIndex(x => x.name == enemySpawn.enemySpawnData[i].name);

            if (index == -1 || isCheck || indexCheck.FindIndex(x => x == index) > -1)
            {
                enemySpawn.enemySpawnData.Clear();

                for (int j = 0; j < backUpList.Count; j++)
                {
                    enemySpawn.enemySpawnData.Add(backUpList[j]);
                }

                break;
            }
            else
            {
                indexCheck.Add(index);
            }
        }

        backUpList.Clear();

        for (int i = 0; i < enemySpawn.enemySpawnData.Count; i++)
        {
            backUpList.Add(enemySpawn.enemySpawnData[i]);
        }
    }

    private void SaveList()
    {
        revertList.Clear();

        for (int i = 0; i < enemySpawnDataList.Count; i++)
        {
            revertList.Add(enemySpawnDataList[i]);
        }
    }

    private void RevertList()
    {
        enemySpawnDataList.Clear();

        for (int i = 0; i < revertList.Count; i++)
        {
            enemySpawnDataList.Add(revertList[i]);
        }
    }

    private void AddData(EnemySpawnData data)
    {
        if (enemySpawnDataList.FindIndex(x => x.name == data.name) > -1)
        {
            return;
        }

        if ((int)data.enemyId >= 100)
        {
            Debug.LogError("이것은 Enemy가 아닙니다. 만약 Enemy라면 값을 변경해주세요. 현재 Enemy < 100");
            return;
        }

        enemySpawnDataList.Add(data);
        enemySpawn.enemySpawnData.Add(data);
        backUpList.Add(data);

        isSave = false;

        Refresh();
        FindText(findText);
        Sort(selectSortIndex);
    }

    private void RemoveData(EnemySpawnData data)
    {
        int index = enemySpawnDataList.FindIndex(x => x.name == data.name);

        if (index > -1)
        {
            enemySpawnDataList.RemoveAt(index);
        }

        index = enemySpawn.enemySpawnData.FindIndex(x => x.name == data.name);

        if (index > -1)
        {
            enemySpawn.enemySpawnData.RemoveAt(index);
        }

        index = backUpList.FindIndex(x => x.name == data.name);

        if (index > -1)
        {
            backUpList.RemoveAt(index);
        }

        isSave = false;

        Refresh();
        FindText(findText);
        Sort(selectSortIndex);
    }

    private void NotSaveEvent()
    {
        if (!isSave)
        {
            if (EditorUtility.DisplayDialog("WARNING! NOT SAVE!", "If you don't save, all the modified data will be lost. Save it now?", "SAVE", "NOT SAVE"))
            {
                Save(isCreateBackUpFile);
            }
        }
    }

    private bool RevertEvent() => isSave ? true : EditorUtility.DisplayDialog("IS REVERT?", "All unsaved data will be revert.", "YES", "NO");

    private void OnEnable()
    {
        enemySpawn = FindObjectOfType<EnemySpawn>();
        serializedObject = new SerializedObject(enemySpawn);
        serializedProperty = serializedObject.FindProperty("enemySpawnData");

        CSVEnemySpawn.Instance.GetData();
        enemySpawnDataList.Clear();

        CSVEnemySpawn.Instance.enemySpawnDatas.Values.ForEach(datas =>
        {
            datas.ForEach(data =>
            {
                enemySpawnDataList.Add(data);
            });
        });

        SaveList();
        FindText(findText);
        Sort(selectSortIndex);
        Refresh();
    }

    private void Update()
    {
        if (lastSelectSortIndex != selectSortIndex)
        {
            lastSelectSortIndex = selectSortIndex;

            Sort(selectSortIndex);
        }

        if (findText != lastFindText)
        {
            lastFindText = findText;

            FindText(findText);
        }

        if (enemySpawn.addSpawnDataQueue.Count > 0)
        {
            AddData(enemySpawn.addSpawnDataQueue.Dequeue());
        }

        if (enemySpawn.removeSpawnDataQueue.Count > 0)
        {
            RemoveData(enemySpawn.removeSpawnDataQueue.Dequeue());
        }

        if (Application.isPlaying != isPlaying)
        {
            isPlaying = Application.isPlaying;
            OnEnable();
        }

        Refresh();
    }

    private void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        EditorGUILayout.Space(5);
        selectSortIndex = EditorGUILayout.Popup("Sort", selectSortIndex, sortName);
        EditorGUILayout.Space(2);
        findText = EditorGUILayout.TextField("Find Text", findText);
        EditorGUILayout.Space(2);
        isReadonly = EditorGUILayout.Toggle("Readonly", isReadonly);
        EditorGUILayout.Space(2);
        isCreateBackUpFile = EditorGUILayout.Toggle("CreateBackUpFile", isCreateBackUpFile);
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            Save(isCreateBackUpFile);
        }

        if (GUILayout.Button("Revert"))
        {
            Revert();
        }

        if (GUILayout.Button("Reload"))
        {
            NotSaveEvent();
            OnEnable();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(10);

        EditorGUIUtility.wideMode = true;
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        EditorGUI.BeginDisabledGroup(isReadonly);
        EditorGUILayout.PropertyField(serializedProperty, new GUIContent("EnemySpawnData"));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndScrollView();
        EditorGUIUtility.wideMode = false;
        EditorGUI.EndDisabledGroup();
    }

    private void OnDestroy()
    {
        NotSaveEvent();
    }
}
#endif