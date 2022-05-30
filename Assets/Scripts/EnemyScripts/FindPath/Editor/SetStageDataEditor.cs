using System.IO;
using UnityEditor;
using UnityEngine;

public class SetStageDataEditor : Editor
{
    public static string getStagePath = Path.Combine("Stage", "StagePrefab", "Stage1");
    public static string stageSavePath = Path.Combine(Application.dataPath, "Resources", "Enemy", "StageData", "Stage1Data");

    [MenuItem("EnemyWindow/SetStageData")]
    public static void Click()
    {
        Debug.Log(Resources.LoadAll<GameObject>(getStagePath)[0].name);
        File.WriteAllText(stageSavePath, "test");
        Debug.Log("Stage Data Reset!");
        Debug.Log("GetStagePath: " + Path.Combine(Application.dataPath, "Resources", getStagePath));
        Debug.Log("StageSavePath: " + stageSavePath);
    }

    /*
     * 1. SetStageData�� ���� ������ ���Ѿ� �Ǵ���
2. ������ ���� �־�� �ϴ���

�ذ�

1. Unity ��ư�� ������ �������� �ݶ��̴��� ���µǰ� �� �ڿ� Resources ������ �������� ������ �־�д�. ( �׸��� ���߿� ���� ���۽� �ҷ��ͼ� ��� )
2. Stage���� ������ �־�� �Ѵ�. ( �̰� ���� ������ ���� �ѵ� �ƴϸ� Tilemap�� ����ص� �ǰ� ) - need : tilemap, limitMinPosition, limitMaxPosition

     */
}
