using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System;

public class TalkManager : MonoSingleton<TalkManager>
{
    #region NPC Talk Panel

    public CanvasGroup talkPanelCvsg;
    public Button nextDialogBtn;
    [SerializeField] private Text talkText;
    [SerializeField] private TextMeshProUGUI nameTmp;

    public float secondPerLit; //�� ���ڴ� �ؽ�Ʈ ��� �ӵ�
    public float durationPerLit; //�ؽ�Ʈ�� �� ��µǰ� �� �� �������� (�� ���ڴ� ��������)
    public float fadeStartTalkDist;  //�÷��̾�� �Ÿ��� �� �̻��� ������ ��ȭâ�� ������������
    [SerializeField] private float autoEndTalkDist; //�÷��̾�� NPC�� �Ÿ��� �� �Ѿ�� �ڵ����� ��ȭ �����ϰ� ����

    private float autoEndTalkDistSquare;
    private float fadeStartTalkDistSquare;
    private float sub;

    public bool IsTalking => CurNPCInfoData != null;
    public NPCInfo CurNPCInfoData { get; private set; }

    private bool isEnding = false; //��ȭ ������ ������ (check end tweening)
    private bool isCompCurDialog; //������ ��簡 �� ��µƴ���
    private int dialogIndex;  //���� ��ȭ�� �ε���
    private int dialogSetIndex; //���� ��ȭ�� �� ��Ʈ�� �ε��� (ID)
    private Transform talkingNPCTr;  //NPC�� Transform
    private TweenCallback twcb1, twcb2;

    #endregion

    #region Subtitle

    public CanvasGroup subCvsg;
    public Text subtitleText;
    private Sequence seq;
    private TweenCallback twcb3;

    #endregion

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
        }; //��ȭ �ؽ�Ʈ�� �� ��µ� �Ŀ�

        twcb2 = () =>
        {
            CurNPCInfoData = null;
            talkText.DOKill();
            talkPanelCvsg.gameObject.SetActive(false);
            isEnding = false;
        }; //��ȭâ alpha�� 0�� �� �Ŀ�

        twcb3 = () =>
        {
            subCvsg.gameObject.SetActive(false);
        }; //�ڸ�(��Ȯ���� �÷��̾� ���� ���� ���â) ��� �� ������ UI����  

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

    public void SetTalkData(NPCInfo data, Transform npcTr, int talkId = -1) //��ȭ ����
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

    public void NextDialog() //���� ��ȭ�� �������ų� ������ ����
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

    public void OnClickNextDialogBtn() //���� ��µǰ� �ִ� ��� �ٷ� �� ��µǰ� ��
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

    public void CompulsoryEndTalk() //��ȭ�߿� �ٸ� NPC�� ��ȭ�� �ؼ� ������ ��ȭ ����
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        talkText.DOKill();
        EventManager.TriggerEvent("TalkWithNPC", false);
    }

    public void EndTalk()  //��ȭ�� �� �����ų� ���� �Ÿ��� ����� ��ȭ����
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

    #region Subtitle
    public void SetSubtitle(string str)
    {
        ResetDialog();
        DOTween.To(() => 0, a => subCvsg.alpha = a, 1, 0.3f);
        seq.Append(subtitleText.DOText(str, secondPerLit * str.Length));
        seq.AppendInterval(durationPerLit * str.Length);
        seq.Append(subCvsg.DOFade(0f, 0.3f));
        seq.AppendCallback(twcb3);
        seq.Play();
    }

    public void SetSubtitle(string[] strs)
    {
        ResetDialog();
        DOTween.To(() => 0, a => subCvsg.alpha = a, 1, 0.3f);

        void SubTxtEmpty() => subtitleText.text = string.Empty;

        for(int i=0; i<strs.Length; i++)
        {
            int si = i;
            seq.Append(subtitleText.DOText(strs[si], secondPerLit * strs[si].Length));
            seq.AppendInterval(durationPerLit*strs[si].Length);
            seq.AppendCallback(SubTxtEmpty);
        }
        seq.Append(subCvsg.DOFade(0f, 0.3f));
        seq.AppendCallback(twcb3).Play();
    }

    private void ResetDialog()
    {
        subCvsg.DOKill();
        subtitleText.DOKill();

        DOTween.Kill("SubtitleDOT");
        seq = DOTween.Sequence();
        seq.SetId("SubtitleDOT");

        subtitleText.text = string.Empty;
        subCvsg.gameObject.SetActive(true);
    }

    #endregion

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
