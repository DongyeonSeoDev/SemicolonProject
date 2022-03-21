#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class EnemySpawnListWindow : EditorWindow
{
    private void OnGUI()
    {
        List<EnemySpawnData> lists = new List<EnemySpawnData>();

        //SerializedObject serializedObject = new SerializedObject(lists);

        //var list = new ReorderableList(,, true, true, true, true);
    }
}
#endif