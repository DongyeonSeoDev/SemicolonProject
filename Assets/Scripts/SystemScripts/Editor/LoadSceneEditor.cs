using UnityEditor;
using UnityEditor.SceneManagement;

public class LoadSceneEditor : EditorWindow
{
    #region Play
    [MenuItem("Play/Title Scene")]
    public static void PlayTitleScene()
    {
        LoadScene("TitleScene");
        EditorApplication.isPlaying = true;
    }
    [MenuItem("Play/Stage Scene")]
    public static void PlayStageScene()
    {
        LoadScene("StageScene");
        EditorApplication.isPlaying = true;
    }
   
    #endregion

    #region Load
    [MenuItem("Load/Title Scene")]
    public static void LoadTitleScene()
    {
        LoadScene("TitleScene");
    }

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

    [MenuItem("Load/Rimuru Scene")]
    public static void LoadSlimeScene()
    {
        LoadScene("RimuruScene");
    }

    

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
