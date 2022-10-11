using UnityEditor;
using UnityEngine;
using Water;
using System.IO;

public class FileUtilWindow : EditorWindow
{
    //�ҷ��� ���̺� ���� ��ġ
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
        GUILayout.Label("(���̺� ������ �������ų� �������ų� �ٲ���� ��,\n �ҷ��� ���� ������ ���� ���� ������ ������ �� ����.)\n(ù��° ���̺� ���ϰ� �ɼ� ���� ������)\n(�⺻ ���� ��ġ : ����ȭ��)", EditorStyles.label);

        GUILayout.Space(8);
        saveFilePath = EditorGUILayout.TextField("Save File Path", saveFilePath);
        if (GUILayout.Button("�ҷ����� (Load Save File)"))
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
        if (GUILayout.Button("����ȭ�� ��� Ȯ��"))
        {
            Debug.Log(System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory));
        }
        if (GUILayout.Button("���� ���̺� ���� ��� Ȯ��"))
        {
            Debug.Log(Application.persistentDataPath);
        }
        if (GUILayout.Button("���� ���̺� ���� ���� ����"))
        {
            System.Diagnostics.Process.Start(Application.persistentDataPath);
        }

        GUILayout.EndScrollView();
    }
}
