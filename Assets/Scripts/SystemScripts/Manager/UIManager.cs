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

    [HideInInspector] public VertexGradient noticeMsgGrd;  // ��� �߾� �˸� �޽��� �⺻ ����

    [HideInInspector] public VertexGradient changeNoticeMsgGrd;  // ��� �߾� �˸� �޽��� �ٲ� ���� ����

    public VertexGradient clearNoticeMsgVGrd;  //Ŭ���� �˸� �޽��� ����

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
                PoolManager.GetItem("NoticeMsg").GetComponent<NoticeMsg>().Set(nus.msg, nus.fontSize, nus.changeVertexGradient, nus.endAction);
            }
        }
    }

    public void InsertNoticeQueue(string msg, float fontSize = 47, bool changeVertexGradient = false, Action endAction = null)
       => noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, changeVertexGradient, endAction));

}