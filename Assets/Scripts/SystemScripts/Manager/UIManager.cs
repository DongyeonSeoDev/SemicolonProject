using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Water;
using System;
using DG.Tweening;

public partial class UIManager : MonoSingleton<UIManager>
{

    private UIMsgQueue rightMoveNoticeMsg = new UIMsgQueue();
    private UIMsgQueue topCenterNoticeMsg = new UIMsgQueue();

    #region Canvas
    private CanvasScaler[] allCanvasScalers;

    public Transform canvasParent;
    public Canvas[] gameCanvases;  //overlay 아닌 애들
    #endregion

    [HideInInspector] public List<InteractionNoticeUI> itrNoticeList = new List<InteractionNoticeUI>();

    public Pair<GameObject, Transform> noticeUIPair;

    public Image topCenterMsgPanel;
    public TextMeshProUGUI topCenterMsgTMP;

    #region VertexGradient
    [HideInInspector] public VertexGradient noticeMsgGrd;  // 상단 중앙 알림 메시지 기본 색상

    public VertexGradient clearNoticeMsgVGrd;  //클리어 알림 메시지 색상
    public VertexGradient bossNoticeMsgVGrd;

    [HideInInspector] public VertexGradient defaultTopCenterMsgVG;  // 상단 중앙 알림 메시지 기본 색상
    #endregion

    public void Notice()
    {
        if(rightMoveNoticeMsg.IsCheckTime)
        {
            if (rightMoveNoticeMsg.CanMakeNotice)
            {
                NoticeUISet nus = rightMoveNoticeMsg.NewNotice;
                PoolManager.GetItem("NoticeMsg").GetComponent<NoticeMsg>().Set(nus);
            }
        }

        if (topCenterNoticeMsg.IsCheckTime)
        {
            if (topCenterNoticeMsg.CanMakeNotice)
            {
                NoticeUISet nus = topCenterNoticeMsg.NewNotice;

                topCenterMsgTMP.color = Color.clear;
                topCenterMsgTMP.colorGradient = nus.changeVertexGradient ? nus.vg : defaultTopCenterMsgVG;
                topCenterMsgTMP.text = nus.msg;
                topCenterMsgTMP.fontSize = nus.fontSize;
                topCenterMsgPanel.gameObject.SetActive(true);
                topCenterMsgTMP.DOColor(Color.white, 0.3f).OnComplete(() =>
                {
                    Util.DelayFunc(() =>
                    {
                        topCenterMsgTMP.DOColor(Color.clear, 0.3f).OnComplete(() => nus.endAction()).SetUpdate(true);
                    }, nus.existTime, this, true, false);
                }).SetUpdate(true);
                //topCenterMsgTMPCvsg.DOFade();
            }
        }
    }

    public void InsertNoticeQueue(string msg, float fontSize = 47, Action endAction = null)
       => rightMoveNoticeMsg.noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, endAction));

    public void InsertNoticeQueue(string msg,  VertexGradient vg, float fontSize = 47, Action endAction = null)
       => rightMoveNoticeMsg.noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, vg, endAction));

    public void InsertTopCenterNoticeQueue(string msg, float fontSize = 65, Action endAction = null, float time = 2f)
       => topCenterNoticeMsg.noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, endAction));

    public void InsertTopCenterNoticeQueue(string msg, VertexGradient vg, float fontSize = 65, Action endAction = null, float time = 2f)
       => topCenterNoticeMsg.noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, vg, endAction));
}