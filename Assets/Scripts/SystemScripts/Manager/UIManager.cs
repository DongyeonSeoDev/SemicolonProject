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
    //public Canvas ordinaryCvs;
    private CanvasScaler[] allCanvasScalers;

    public Transform canvasParent;
    public Canvas[] gameCanvases;  //overlay 아닌 애들
    #endregion

    [HideInInspector] public List<Menu> gameMenuList = new List<Menu>();
    [HideInInspector] public List<InteractionNoticeUI> itrNoticeList = new List<InteractionNoticeUI>();

    public Pair<GameObject, Transform> noticeUIPair;
    public Pair<GameObject, Transform> interactionMarkPair;

    public Image topCenterMsgPanel;
    public TextMeshProUGUI topCenterMsgTMP;
    //private CanvasGroup topCenterMsgTMPCvsg;

    #region VertexGradient
    [HideInInspector] public VertexGradient noticeMsgGrd;  // 상단 중앙 알림 메시지 기본 색상

    public VertexGradient clearNoticeMsgVGrd;  //클리어 알림 메시지 색상
    public VertexGradient bossNoticeMsgVGrd;

    [HideInInspector] public VertexGradient defaultTopCenterMsgVG;  // 상단 중앙 알림 메시지 기본 색상
    #endregion

    public void Notice()
    {
        if(rightMoveNoticeMsg.noticeCheckElapsed < Time.unscaledTime)
        {
            rightMoveNoticeMsg.noticeCheckElapsed = Time.unscaledTime + 0.25f;

            if(rightMoveNoticeMsg.noticeQueue.Count > 0 && !rightMoveNoticeMsg.isNoticing)
            {
                rightMoveNoticeMsg.isNoticing = true;
                NoticeUISet nus = rightMoveNoticeMsg.noticeQueue.Dequeue();
                nus.endAction += () => rightMoveNoticeMsg.isNoticing = false;
                PoolManager.GetItem("NoticeMsg").GetComponent<NoticeMsg>().Set(nus);
            }
        }

        if(topCenterNoticeMsg.noticeCheckElapsed < Time.unscaledTime)
        {
            topCenterNoticeMsg.noticeCheckElapsed = Time.unscaledTime + 0.1f;
            
            if(topCenterNoticeMsg.noticeQueue.Count > 0 && !topCenterNoticeMsg.isNoticing)
            {
                topCenterNoticeMsg.isNoticing = true;
                NoticeUISet nus = topCenterNoticeMsg.noticeQueue.Dequeue();
                nus.endAction += () => topCenterNoticeMsg.isNoticing = false;

                //topCenterMsgTMPCvsg.alpha = 0;
                topCenterMsgTMP.color = Color.clear;
                topCenterMsgTMP.colorGradient = nus.changeVertexGradient ? nus.vg : defaultTopCenterMsgVG;
                topCenterMsgTMP.text = nus.msg;
                topCenterMsgTMP.fontSize = nus.fontSize;
                topCenterMsgPanel.gameObject.SetActive(true);
                topCenterMsgTMP.DOColor(Color.white, 0.5f).OnComplete(() =>
                {
                    Util.DelayFunc(() =>
                    {
                        topCenterMsgTMP.DOColor(Color.clear, 0.5f).OnComplete(() => nus.endAction()).SetUpdate(true);
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

    public void InsertTopCenterNoticeQueue(string msg, float fontSize = 65, Action endAction = null, float time = 3f)
       => topCenterNoticeMsg.noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, endAction));

    public void InsertTopCenterNoticeQueue(string msg, VertexGradient vg, float fontSize = 65, Action endAction = null, float time = 3f)
       => topCenterNoticeMsg.noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, vg, endAction));
}