using UnityEditor;
using UnityEditor.SceneManagement;

public class LoadSceneEditor : EditorWindow
{
    #region Play
    [MenuItem("Play/Stage Scene")]
    public static void PlayStageScene()
    {
        LoadScene("StageScene");
        EditorApplication.isPlaying = true;
    }
    #endregion

    #region Load
    [MenuItem("Load/Stage Scene")]
    public static void LoadStageScene()
    {
        LoadScene("StageScene");
    }

    [MenuItem("Load/System Scene")]
    public static void LoadSystemScene()
    {
        LoadScene("SystemScene");
    }

    [MenuItem("Load/Enemy Scene")]
    public static void LoadEnemyScene()
    {
        LoadScene("EnemyScene");
    }

    [MenuItem("Load/Slime Scene")]
    public static void LoadSlimeScene()
    {
        LoadScene("SlimeScene");
    }

    [MenuItem("Load/Stage Test Scene")]
    public static void LoadStageTest()
    {
        LoadScene("StageTestScene");
    }

   /* [MenuItem("Load/Boss Test Scene")]
    public static void LoadBossTest()
    {
        LoadScene("BossTestScene");
    }*/
    #endregion

    private static void LoadScene(string sceneName)
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        EditorSceneManager.OpenScene("Assets/Scenes/" + sceneName + ".unity");
    }
}
