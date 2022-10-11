using UnityEngine;
using UnityEngine.UI;

public class NextStageIcon : MonoBehaviour
{
    private StageDataSO stageData;

    public DoorDirType doorDirType;
    public Image iconImg;    

    public void SetData(StageDataSO data)
    {
        stageData = data;

        if(stageData)
        {
            if(stageData.areaType == AreaType.MONSTER || stageData.areaType == AreaType.BOSS)
            {
                iconImg.sprite = MonsterCollection.Instance.GetMonsterInfo(stageData.enemySpeciesArea.ToString()).bodyImg;
            }
            else
            {
                iconImg.sprite = StageManager.Instance.areaSprDict[stageData.areaType];
            }
        }
    }
}
