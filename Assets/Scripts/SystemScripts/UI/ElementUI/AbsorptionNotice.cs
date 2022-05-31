using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AbsorptionNotice : MonoBehaviour
{
    public Pair<Image, Text> mobImgNamePair;  //몬스터 이미지와 이름 텍스트
    public Triple<Image, Text, Text> absorptionResultTriple; //흡수율 게이지, 흡수율 수치 (%), 결과 (성공 or 실패)

    private AbsorptionData currentAbpData;

    private CanvasGroup cvsg;
    private RectTransform rt;
    private Vector2 startPos;
    private Vector2 target;
    private Vector2 rightAnglePos;

    private bool isMove;
    private float moveTime;

    private bool isTweening;

    private int slotIdx;
    private bool isAssimNotice;

    private void Awake()
    {
        rt = GetComponent<RectTransform>(); 
        cvsg = GetComponent<CanvasGroup>();
    }

    public void ShowNotice(Vector2 start, Vector2 target, AbsorptionData data, int index)
    {
        currentAbpData = data;
        startPos = start;
        this.target = target;
        rightAnglePos = Global.GetRightAngleCoord(target, start, true);
        slotIdx = index;

        rt.anchoredPosition = start;
        cvsg.alpha = 0f;
        cvsg.DOFade(1f, 0.3f);

        ChangeBodyData cbd = MonsterCollection.Instance.GetMonsterInfo(data.mobId);
        mobImgNamePair.first.sprite = cbd.bodyImg;
        mobImgNamePair.second.text = cbd.bodyName;
        absorptionResultTriple.first.fillAmount = 0f;
        absorptionResultTriple.second.text = "0%";

        isAssimNotice = BattleUIManager.Instance.HasBody(data.mobId);

        if (!isAssimNotice)  //아직 슬롯에 없는 몹 흡수함
        {
            if (data.absorptionSuccess)
            {
                absorptionResultTriple.third.text = "성공";
                absorptionResultTriple.third.color = Color.green;
            }
            else
            {
                absorptionResultTriple.third.text = "실패";
                absorptionResultTriple.third.color = Color.red;
            } 
        }
        else  //슬롯에 있는 몹 조지고 동화율 오름
        {
            absorptionResultTriple.third.text = "동화율 상승";
            absorptionResultTriple.third.color = Color.green;
        }

        moveTime = 0f;
        isMove = true;
        isTweening = false;
    }

    private void Update()
    {
        if (isMove)
        {
            moveTime += Time.deltaTime * 1.5f;
            Vector2 p1 = Vector2.Lerp(startPos, rightAnglePos, moveTime);
            Vector2 p2 = Vector2.Lerp(rightAnglePos, target, moveTime);
            Vector2 p3 = Vector2.Lerp(p1, p2, moveTime);
            rt.anchoredPosition = p3;
            
            if(moveTime > 1f)
            {
                BattleUIManager.Instance.IsMoving = false;
                rt.anchoredPosition = target;
                isMove = false;
                isTweening = true;
            }
        }

        if (isTweening)
        {
            float curRate = absorptionResultTriple.first.fillAmount * 100f;
            absorptionResultTriple.first.fillAmount += Time.deltaTime * 0.4f;
            absorptionResultTriple.second.text = string.Concat((int)curRate, '%');

            if (isAssimNotice)  //동화율 알림창이면
            {
                if(curRate >= currentAbpData.assimilationRate)
                {
                    absorptionResultTriple.first.fillAmount = currentAbpData.assimilationRate * 0.01f;
                    absorptionResultTriple.second.text = $"{currentAbpData.assimilationRate}%";
                    isTweening = false;

                    Util.DelayFunc(Exit, 2f, this, false, false);
                }
            }
            else  //흡수율 알림창이면
            {
                if (curRate >= currentAbpData.absorptionRate)
                {
                    absorptionResultTriple.first.fillAmount = currentAbpData.absorptionRate * 0.01f;
                    absorptionResultTriple.second.text = $"{currentAbpData.absorptionRate}%";
                    isTweening = false;

                    Util.DelayFunc(Exit, 2f, this, false, false);
                }
            }
        }
    }

    private void Exit()
    {
        BattleUIManager.Instance.InsertEndedNotice(this);
    }
}
