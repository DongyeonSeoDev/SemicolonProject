using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Water;
using System;

public partial class UIManager : MonoSingleton<UIManager>
{
    private Queue<NoticeUISet> noticeQueue = new Queue<NoticeUISet>();

    private bool isNoticing = false;
    private float noticeCheckElapsed;

    #region Canvas
    //public Canvas ordinaryCvs;
    public Canvas[] gameCanvases;
    #endregion

    [HideInInspector] public List<Menu> gameMenuList = new List<Menu>();
    [HideInInspector] public List<InteractionNoticeUI> itrNoticeList = new List<InteractionNoticeUI>();

    public Pair<GameObject, Transform> noticeUIPair;
    public Pair<GameObject, Transform> interactionMarkPair;

    [HideInInspector] public VertexGradient noticeMsgGrd;  // 상단 중앙 알림 메시지 기본 색상

    public VertexGradient clearNoticeMsgVGrd;  //클리어 알림 메시지 색상

    private void Notice()
    {
        if(noticeCheckElapsed < Time.time)
        {
            noticeCheckElapsed += 0.25f;

            if(noticeQueue.Count > 0 && !isNoticing)
            {
                isNoticing = true;
                NoticeUISet nus = noticeQueue.Dequeue();
                nus.endAction += () => isNoticing = false;
                PoolManager.GetItem("NoticeMsg").GetComponent<NoticeMsg>().Set(nus);
            }
        }
    }

    public void InsertNoticeQueue(string msg, float fontSize = 47, Action endAction = null)
       => noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, endAction));

    public void InsertNoticeQueue(string msg,  VertexGradient vg, float fontSize = 47, Action endAction = null)
       => noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, vg, endAction));
}