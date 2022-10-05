using UnityEditor;
using UnityEngine;
using Water;
using System.IO;

public class FileUtilWindow : EditorWindow
{
    //불러올 세이브 파일 위치
    private string saveFilePath;

    private Vector2 scrollPos = Vector2.zero;

    [MenuItem("FileUtil/Normal")]
    public static void ShowFileUtilWindow()
    {
        GetWindow(typeof(FileUtilWindow));
    }

    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.MinWidth(200), GUILayout.MaxWidth(1000), GUILayout.ExpandWidth(true), GUILayout.MinHeight(200), GUILayout.MaxHeight(1000), GUILayout.ExpandHeight(true));

        GUILayout.Label("[Page]", EditorStyles.boldLabel);
        if (GUILayout.Button("Open GitHub Page"))
        {
            Application.OpenURL("https://github.com/DongyeonSeoDev/SemicolonProject");
        }

        GUILayout.Space(25);

        GUILayout.Label("[Rollback Data]", EditorStyles.boldLabel);
        GUILayout.Label("(세이브 데이터 망가지거나 없어지거나 바꿔야할 때,\n 불러올 파일 있으면 정상 저장 파일을 가져올 수 있음.)\n(첫번째 세이브 파일과 옵션 파일 가져옴)\n(기본 저장 위치 : 바탕화면)", EditorStyles.label);

        GUILayout.Space(8);
        saveFilePath = EditorGUILayout.TextField("Save File Path", saveFilePath);
        if (GUILayout.Button("불러오기 (Load Save File)"))
        {
            if (string.IsNullOrEmpty(saveFilePath)) saveFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

            /*File.Delete(Global.GAME_SAVE_FILE.PersistentDataPath());
            File.Delete(Global.SAVE_FILE_1.PersistentDataPath());
            File.Delete(Global.SAVE_FILE_2.PersistentDataPath());
            File.Delete(Global.SAVE_FILE_3.PersistentDataPath());
            File.Delete(SaveFileStream.EternalOptionSaveFileName.PersistentDataPath());*/

            string sf = File.ReadAllText(string.Concat(saveFilePath, '/', Global.SAVE_FILE_1));
            File.WriteAllText(Global.SAVE_FILE_1.PersistentDataPath(), sf);
            sf = File.ReadAllText(string.Concat(saveFilePath, '/', SaveFileStream.EternalOptionSaveFileName));
            File.WriteAllText(SaveFileStream.EternalOptionSaveFileName.PersistentDataPath(), sf);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("바탕화면 경로 확인"))
        {
            Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory));
        }
        if (GUILayout.Button("게임 세이브 파일 경로 확인"))
        {
            Debug.Log(Application.persistentDataPath);
        }
        if (GUILayout.Button("게임 세이브 파일 폴더 열기"))
        {
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }

        GUILayout.EndScrollView();
    }
}
