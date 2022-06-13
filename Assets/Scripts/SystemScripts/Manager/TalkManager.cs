using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

public class TalkManager : MonoSingleton<TalkManager>
{
    public CanvasGroup talkPanelCvsg;
    public Button nextDialogBtn;
    [SerializeField] private Text talkText;
    [SerializeField] private TextMeshProUGUI nameTmp;

    public float secondPerLit; //한 문자당 텍스트 출력 속도
    public float durationPerLit; //텍스트가 다 출력되고 몇 초 보여줄지 (한 문자당 기준으로)
    public float fadeStartTalkDist;  //플레이어와 거리가 몇 이상일 때부터 대화창이 투명해지는지
    [SerializeField] private float autoEndTalkDist; //플레이어와 NPC의 거리가 몇 넘어가면 자동으로 대화 종료하게 할지

    private float autoEndTalkDistSquare;
    private float fadeStartTalkDistSquare;
    private float sub;

    public bool IsTalking => CurNPCInfoData != null;
    public NPCInfo CurNPCInfoData { get; private set; }

    private bool isEnding = false; //대화 끝내는 중인지 (check end tweening)
    private bool isCompCurDialog; //현재의 대사가 다 출력됐는지
    private int dialogIndex;  //현재 대화의 인덱스
    private int dialogSetIndex; //현재 대화의 한 세트의 인덱스 (ID)
    private Transform talkingNPCTr;  //NPC의 Transform
    private TweenCallback twcb1, twcb2; 

    private IEnumerator delayCoroutine = null;

    private void Awake()
    {
        autoEndTalkDistSquare = autoEndTalkDist * autoEndTalkDist;
        fadeStartTalkDistSquare = fadeStartTalkDist * fadeStartTalkDist;
        sub = autoEndTalkDistSquare - fadeStartTalkDistSquare;

        twcb1 = () =>
        {
            //CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value[dialogIndex].talkEndEvent?.Invoke();
            isCompCurDialog = true;
            CurNPCInfoData.talkContents[dialogSetIndex].value[dialogIndex].talkEndEventKey.TriggerEvent();
            DelayFunc(NextDialog, CurNPCInfoData.talkContents[dialogSetIndex].value[dialogIndex].message.Length * durationPerLit);
        }; //대화 텍스트가 다 출력된 후에

        twcb2 = () =>
        {
            CurNPCInfoData = null;
            talkText.DOKill();
            talkPanelCvsg.gameObject.SetActive(false);
            isEnding = false;
        }; //대화창 alpha가 0이 된 후에

        EventManager.StartListening("TalkWithNPC", (Action<bool>) (talkStart =>
        {
            Global.CurrentPlayer.GetComponent<PlayerInput>().IsPauseByCutScene = talkStart;
            nextDialogBtn.gameObject.SetActive(talkStart);
        }));
    }

    private void Update()
    {
        if (IsTalking)
        {
            float sDist = (talkingNPCTr.position - Global.GetSlimePos.position).sqrMagnitude;
            if (sDist > autoEndTalkDistSquare)
            {
                EndTalk();
            }
            else
            {
                if(sDist > fadeStartTalkDistSquare)
                {
                    talkPanelCvsg.alpha = Mathf.Lerp(0, 1, 1f - (sDist - fadeStartTalkDistSquare) / sub);
                }
            }
        }
    }

    public void SetTalkData(NPCInfo data, Transform npcTr, int talkId = -1) //대화 시작
    {
        if (isEnding) return;

        if(IsTalking)
        {
            if (npcTr == talkingNPCTr) return;

            CompulsoryEndTalk();
        }

        talkingNPCTr = npcTr;
        CurNPCInfoData = data;

        if(talkId < 0)
        {
            dialogSetIndex = CurNPCInfoData.talkId;
        }
        else
        {
            dialogSetIndex = talkId;
        }

        talkPanelCvsg.alpha = 1;
        talkPanelCvsg.gameObject.SetActive(true);

        dialogIndex = -1;
        isCompCurDialog = false;
        EventManager.TriggerEvent("TalkWithNPC", true);

        nameTmp.SetText(CurNPCInfoData.npcName);
        NextDialog();
    }

    public void NextDialog() //다음 대화를 가져오거나 없으면 끝냄
    {
        isCompCurDialog = false;
        talkText.text = string.Empty;
        if (++dialogIndex < CurNPCInfoData.talkContents[dialogSetIndex].value.Count)
        {
            string msg = CurNPCInfoData.talkContents[dialogSetIndex].value[dialogIndex].message;
            talkText.DOText(msg, msg.Length * secondPerLit).OnComplete(twcb1);
        }
        else
        {
            EndTalk();
        }
    }

    public void OnClickNextDialogBtn() //현재 출력되고 있는 대사 바로 다 출력되게 함
    {
        if (!isCompCurDialog)
        {
            talkText.DOKill();
            talkText.text = CurNPCInfoData.talkContents[dialogSetIndex].value[dialogIndex].message;
            CurNPCInfoData.talkContents[dialogSetIndex].value[dialogIndex].talkEndEventKey.TriggerEvent();
            DelayFunc(NextDialog, CurNPCInfoData.talkContents[dialogSetIndex].value[dialogIndex].message.Length * durationPerLit);

            isCompCurDialog = true;
        }
        else
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
            NextDialog();
        }
    }

    public void CompulsoryEndTalk() //대화중에 다른 NPC와 대화를 해서 강제로 대화 종료
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        talkText.DOKill();
        EventManager.TriggerEvent("TalkWithNPC", false);
    }

    public void EndTalk()  //대화가 다 끝나거나 일정 거리를 벗어나서 대화종료
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        isEnding = true;
        talkPanelCvsg.DOFade(0, 0.4f).OnComplete(twcb2);
        EventManager.TriggerEvent("TalkWithNPC", false);
    }

    private void DelayFunc(Action func, float delay)
    {
        if(delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        delayCoroutine = DelayCo(func, delay);
        StartCoroutine(delayCoroutine);
    }

    private IEnumerator DelayCo(Action func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func();
    }
}
