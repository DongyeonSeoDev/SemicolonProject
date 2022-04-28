
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TalkManager : MonoSingleton<TalkManager>
{
    public CanvasGroup talkPanelCvsg;
    [SerializeField] private Text talkText;

    public float secondPerLit; //한 문자당 텍스트 출력 속도
    public float durationPerLit; //텍스트가 다 출력되고 몇 초 보여줄지 (한 문자당 기준으로)
    [SerializeField] private float autoEndTalkDist; //플레이어와 NPC의 거리가 몇 넘어가면 자동으로 대화 종료하게 할지
    private float autoEndTalkDistSquare;

    public bool IsTalking => CurNPCInfoData != null;
    public NPCInfo CurNPCInfoData { get; private set; }

    private int dialogIndex;
    private Transform talkingNPCTr;
    private TweenCallback twcb;

    private IEnumerator delayCoroutine = null;

    private void Awake()
    {
        autoEndTalkDistSquare = autoEndTalkDist * autoEndTalkDist;

        twcb = () =>
        {
            DelayFunc(NextDialog, CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value[dialogIndex].message.Length * durationPerLit);
        };
    }

    private void Update()
    {
        if (IsTalking)
        {
            if ((talkingNPCTr.position - Global.GetSlimePos.position).sqrMagnitude > autoEndTalkDistSquare)
            {
                EndTalk();
            }
        }
    }

    public void SetTalkData(NPCInfo data, Transform npcTr)
    {
        if(IsTalking)
        {
            EndTalk();
        }

        talkText.DOKill();
        talkPanelCvsg.DOKill();

        talkingNPCTr = npcTr;
        CurNPCInfoData = data;

        talkPanelCvsg.alpha = 1;
        talkPanelCvsg.gameObject.SetActive(true);

        dialogIndex = -1;

        NextDialog();
    }

    public void NextDialog()
    {
        talkText.text = string.Empty;
        if (++dialogIndex < CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value.Count)
        {
            string msg = CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value[dialogIndex].message;
            talkText.DOText(msg, msg.Length * secondPerLit).OnComplete(twcb);
        }
        else
        {
            EndTalk();
        }
    }

    public void EndTalk()
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        CurNPCInfoData = null;
        talkText.DOKill();
        talkPanelCvsg.DOKill();
        talkPanelCvsg.DOFade(0, 0.4f).OnComplete(() => talkPanelCvsg.gameObject.SetActive(false));
    }

    private void DelayFunc(System.Action func, float delay)
    {
        if(delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        delayCoroutine = DelayCo(func, delay);
        StartCoroutine(delayCoroutine);
    }

    private IEnumerator DelayCo(System.Action func, float delay)
    {
        yield return new WaitForSeconds(delay);
        func();
    }
}
