using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AbsorptionNotice : MonoBehaviour
{
    public Pair<Image, Text> mobImgNamePair;  //몬스터 이미지와 이름 텍스트
    public Triple<Image, Text, Text> absorptionResultTriple; //흡수율 게이지, 흡수율 수치 (%), 결과 (성공 or 실패)
    public Image assimSecondFill;  //동화율 게이지 두번째 (101% 이상을 표시하는)

    private AbsorptionData currentAbpData;

    private CanvasGroup cvsg;
    private RectTransform rt;
    private Vector2 startPos;
    private Vector2 target;
    private Vector2 rightAnglePos;

    private bool isMove;  //있어야 할 위치로 가는중?
    private float moveTime;  

    private bool isTweening;  //게이지 차고 있냐

    //private int slotIdx;

    private KillNoticeType killNoticeType;

    private void Awake()
    {
        rt = GetComponent<RectTransform>(); 
        cvsg = GetComponent<CanvasGroup>();
    }

    public void ShowNotice(Vector2 start, Vector2 target, AbsorptionData data)
    {
        currentAbpData = data;
        startPos = start;
        this.target = target;
        rightAnglePos = Global.GetRightAngleCoord(target, start, true);
        //slotIdx = index;

        rt.anchoredPosition = start;
        cvsg.alpha = 0f;
        cvsg.DOFade(1f, 0.3f);

        ChangeBodyData cbd = MonsterCollection.Instance.GetMonsterInfo(data.mobId);
        mobImgNamePair.first.sprite = cbd.bodyImg;
        mobImgNamePair.second.text = cbd.bodyName;
        absorptionResultTriple.first.fillAmount = 0f;
        assimSecondFill.fillAmount = 0f;
        absorptionResultTriple.second.text = "0%";

        killNoticeType = data.killNoticeType;

        switch(killNoticeType)
        {
            case KillNoticeType.FAIL:
                absorptionResultTriple.third.text = "실패";
                absorptionResultTriple.third.color = Color.red;
                absorptionResultTriple.first.color = Util.Change255To1Color(155, 236, 48, 255);
                break;
            case KillNoticeType.SUCCESS:
                absorptionResultTriple.third.text = "성공";
                absorptionResultTriple.third.color = Color.green;
                absorptionResultTriple.first.color = Util.Change255To1Color(155, 236, 48, 255);
                break;
            case KillNoticeType.ALREADY:
                absorptionResultTriple.third.text = "흡수완료";
                absorptionResultTriple.third.color = Util.Change255To1Color(157, 157, 157, 255);
                break;
            case KillNoticeType.UNDERSTANDING:
                absorptionResultTriple.third.text = "동화율 상승";
                absorptionResultTriple.third.color = Util.Change255To1Color(83, 127, 241, 255);
                absorptionResultTriple.first.color = Util.Change255To1Color(48, 85, 236, 255);
                break;
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
            switch (killNoticeType)
            {
                case KillNoticeType.ALREADY:
                    Util.DelayFunc(Exit, 2f, this, false, false);
                    isTweening = false;
                    break;
                case KillNoticeType.UNDERSTANDING:
                    if (currentAbpData.assimilationRate > 100f)
                    {
                        TwoFillRise(currentAbpData.assimilationRate);
                    }
                    else
                    {
                        FillRise(currentAbpData.assimilationRate);
                    }
                    break;
                default:
                    FillRise(currentAbpData.absorptionRate);  
                    break;
            }
        }
    }

    private void FillRise(float max)
    {
        absorptionResultTriple.first.fillAmount += Time.deltaTime * 0.4f;
        float curRate = absorptionResultTriple.first.fillAmount * 100f;
        absorptionResultTriple.second.text = string.Concat((int)curRate, '%');

        if (curRate >= max)
        {
            absorptionResultTriple.first.fillAmount = max * 0.01f;
            absorptionResultTriple.second.text = $"{(int)max}%";
            isTweening = false;

            Util.DelayFunc(Exit, 2f, this, false, false);
        }
    }

    private void TwoFillRise(float max)
    {
        float curRate;
        if (absorptionResultTriple.first.fillAmount < 1f)
        {
            absorptionResultTriple.first.fillAmount += Time.deltaTime * 0.4f;
            curRate = absorptionResultTriple.first.fillAmount * 100f;
        }
        else
        {
            assimSecondFill.fillAmount += Time.deltaTime * 0.4f;
            curRate = 100f + assimSecondFill.fillAmount * 100f;
        }
        absorptionResultTriple.second.text = string.Concat((int)curRate, '%');

        if(curRate >= max)
        {
            isTweening = false;
            absorptionResultTriple.second.text = $"{(int)max}%";
            Util.DelayFunc(Exit, 2f, this, false, false);
        }
    }

    private void Exit()
    {
        BattleUIManager.Instance.InsertEndedNotice(this);
    }
}
