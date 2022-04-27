using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TalkManager : MonoBehaviour
{
    public CanvasGroup talkPanelCvsg;
    [SerializeField] private Text talkText;

    public float secondPerLit;
    public float durationPerLit;

    public bool IsTalking => CurNPCInfoData != null;
    public NPCInfo CurNPCInfoData { get; private set; }

    private int dialogIndex;
    private Transform talkingNPCTr;
    private TweenCallback twcb;

    private void Awake()
    {
        System.Action tempAction = () =>
        {
            talkText.text = string.Empty;
            NextDialog();
        };

        twcb = () =>
        {
            Util.DelayFunc(tempAction, CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value[dialogIndex].message.Length * durationPerLit, this);
        };
    }

    private void Update()
    {
        
    }

    public void SetTalkData(NPCInfo data, Transform npcTr)
    {
        talkingNPCTr = npcTr;
        CurNPCInfoData = data;
        talkPanelCvsg.alpha = 1;
        talkPanelCvsg.gameObject.SetActive(true);
        dialogIndex = -1;

        NextDialog();
    }

    public void NextDialog()
    {
        if (++dialogIndex < CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value.Count)
        {
            string msg = CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value[dialogIndex].message;
            talkText.DOText(msg, msg.Length * secondPerLit).OnComplete(twcb);
        }
        else
        {
            Util.DelayFunc(EndTalk, CurNPCInfoData.talkContents[CurNPCInfoData.talkId].value[dialogIndex].message.Length * durationPerLit, this);
        }
    }

    public void EndTalk()
    {
        CurNPCInfoData = null;
        talkPanelCvsg.DOFade(0, 0.4f).OnComplete(() => talkPanelCvsg.gameObject.SetActive(false));
    }
}
