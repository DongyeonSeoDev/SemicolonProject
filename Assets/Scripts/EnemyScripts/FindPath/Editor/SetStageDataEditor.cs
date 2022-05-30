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
     * 1. SetStageData를 언제 실행을 시켜야 되는지
2. 변수를 언제 넣어야 하는지

해결

1. Unity 버튼을 누르면 스테이지 콜라이더가 리셋되게 한 뒤에 Resources 폴더에 스테이지 정보를 넣어둔다. ( 그리고 나중에 게임 시작시 불러와서 사용 )
2. Stage에서 가지고 있어야 한다. ( 이게 제일 나을것 같긴 한데 아니면 Tilemap을 사용해도 되고 ) - need : tilemap, limitMinPosition, limitMaxPosition

     */
}
