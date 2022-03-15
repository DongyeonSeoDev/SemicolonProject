using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Water;
using System;
using DG.Tweening;

public partial class UIManager : MonoSingleton<UIManager>
{
    //음... 이거 나중에 좀 많아진다싶으면 새 로직 써야겠다
    private Queue<NoticeUISet> noticeQueue = new Queue<NoticeUISet>();
    private Queue<NoticeUISet> topCenterMsgQueue = new Queue<NoticeUISet>();

    private bool isNoticing = false, isTCNoticing = false;
    private float noticeCheckElapsed, topCenterNoticeCheckElapsed;

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

    [HideInInspector] public VertexGradient noticeMsgGrd;  // 상단 중앙 알림 메시지 기본 색상

    public VertexGradient clearNoticeMsgVGrd;  //클리어 알림 메시지 색상

    [HideInInspector] public VertexGradient defaultTopCenterMsgVG;  // 상단 중앙 알림 메시지 기본 색상

    private void Notice()
    {
        if(noticeCheckElapsed < Time.time)
        {
            noticeCheckElapsed = Time.time + 0.25f;

            if(noticeQueue.Count > 0 && !isNoticing)
            {
                isNoticing = true;
                NoticeUISet nus = noticeQueue.Dequeue();
                nus.endAction += () => isNoticing = false;
                PoolManager.GetItem("NoticeMsg").GetComponent<NoticeMsg>().Set(nus);
            }
        }

        if(topCenterNoticeCheckElapsed < Time.time)
        {
            topCenterNoticeCheckElapsed = Time.time + 0.1f;
            
            if(topCenterMsgQueue.Count > 0 && !isTCNoticing)
            {
                isTCNoticing = true;
                NoticeUISet nus = topCenterMsgQueue.Dequeue();
                nus.endAction += () => isTCNoticing = false;

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
                        topCenterMsgTMP.DOColor(Color.clear, 0.5f).OnComplete(() => nus.endAction());
                    }, nus.existTime, this, true);
                });
                //topCenterMsgTMPCvsg.DOFade();
            }
        }
    }

    public void InsertNoticeQueue(string msg, float fontSize = 47, Action endAction = null)
       => noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, endAction));

    public void InsertNoticeQueue(string msg,  VertexGradient vg, float fontSize = 47, Action endAction = null)
       => noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, vg, endAction));

    public void InsertTopCenterNoticeQueue(string msg, float fontSize = 65, Action endAction = null, float time = 3f)
       => topCenterMsgQueue.Enqueue(new NoticeUISet(msg, fontSize, endAction));

    public void InsertTopCenterNoticeQueue(string msg, VertexGradient vg, float fontSize = 65, Action endAction = null, float time = 3f)
       => topCenterMsgQueue.Enqueue(new NoticeUISet(msg, fontSize, vg, endAction));
}