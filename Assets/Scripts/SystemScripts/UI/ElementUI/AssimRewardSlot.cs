using UnityEngine;
using UnityEngine.UI;

public class AssimRewardSlot : MonoBehaviour
{
    public Triple<Image, Text, Text> monsterInfoUI;
    public Button decisionBtn;

    public void Set(ChangeBodyData data, int up)
    {
        decisionBtn.onClick.RemoveAllListeners();   

        monsterInfoUI.first.sprite = data.bodyImg;
        monsterInfoUI.second.text = data.bodyName;
        monsterInfoUI.third.text = $"µ¿È­À² : {PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(data.bodyId.ToString())}%";

        decisionBtn.onClick.AddListener(() => MonsterCollection.Instance.MonsterAssimReward(up, data.bodyId.ToString()));
    }
}
