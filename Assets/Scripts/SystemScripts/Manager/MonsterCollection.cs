using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MonsterCollection : MonoSingleton<MonsterCollection>
{
    private PlayerEnemyUnderstandingRateManager urmg;

    private Dictionary<string, MonsterInfoSlot> mobIdToSlot = new Dictionary<string, MonsterInfoSlot>();

    public Pair<GameObject, Transform> mobInfoUIPair;

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;

        //mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = monsterCount / 3 + 1;

        //mobIdToSlot�� Ű�� �� �ְ� mobInfoUIPair�� UI�����ϰ� ���� �ֱ�
        //����� �� ������ �ҷ���
        AllUpdateCollection();
    }

    public void UpdateCollection(string id)
    {
        //PlayerEnemyUnderstandingRateManager���� playerEnemyUnderStandingRateDic�� changableBodyList���� �� �ְ� �ϴ� ���� �ʿ�. ChangeBodyData�� �� ����, �̹���, �̸� �ʿ�
        //mobIdToSlot[id].UpdateRate((float)urmg.playerEnemyUnderStandingRateDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateCollection()
    {

    }
}
