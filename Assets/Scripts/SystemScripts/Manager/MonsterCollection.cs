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

        //mobIdToSlot에 키와 값 넣고 mobInfoUIPair로 UI생성하고 값들 넣기
        //저장된 값 있으면 불러옴
        AllUpdateCollection();
    }

    public void UpdateCollection(string id)
    {
        //PlayerEnemyUnderstandingRateManager에서 playerEnemyUnderStandingRateDic와 changableBodyList읽을 수 있게 하는 변수 필요. ChangeBodyData에 몹 설명, 이미지, 이름 필요
        //mobIdToSlot[id].UpdateRate((float)urmg.playerEnemyUnderStandingRateDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateCollection()
    {

    }
}
