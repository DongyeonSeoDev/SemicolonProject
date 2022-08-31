using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AssimRewardPanel : MonoBehaviour
{
    public CanvasGroup cvsg;

    public Text topText;

    public List<AssimRewardSlot> slots;

    public void Open(List<string> monsterIDList, int up)
    {
        TimeManager.TimePause();
        cvsg.alpha = 0;
        transform.localScale = SVector3.zeroPointSeven;
        topText.text = string.Format("어떤 몬스터의 동화율({0}%)을 올리겠습니까?", up);

        for (int i = 0; i < slots.Count; i++) slots[i].Set(MonsterCollection.Instance.GetMonsterInfo(monsterIDList[i]), up);

        gameObject.SetActive(true);

        cvsg.DOFade(1f, 0.3f).SetUpdate(true);
        transform.DOScale(Vector3.one, 0.35f).SetUpdate(true);
    }

    public void Close()
    {
        transform.DOScale(SVector3.zeroPointSeven, 0.3f).SetUpdate(true);
        cvsg.DOFade(0f, 0.35f).SetUpdate(true).OnComplete(() =>
        {
            TimeManager.TimeResume();
            gameObject.SetActive(false);
        });
    }
}
