using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Water;
using System;
using UnityEngine.Events;

public partial class UIManager : MonoSingleton<UIManager>
{
    private Queue<NoticeUISet> noticeQueue = new Queue<NoticeUISet>();

    private bool isNoticing = false;
    private float noticeCheckElapsed;

    [HideInInspector] public List<Menu> gameMenuList = new List<Menu>();

    public Pair<GameObject, Transform> noticeUIPair;

    public VertexGradient noticeMsgGrd;

    private void Notice()
    {
        if(noticeCheckElapsed < Time.time)
        {
            noticeCheckElapsed += 0.15f;

            if(noticeQueue.Count > 0 && !isNoticing)
            {
                isNoticing = true;
                NoticeUISet nus = noticeQueue.Dequeue();
                nus.endAction += () => isNoticing = false;
                PoolManager.GetItem("NoticeMsg").GetComponent<NoticeMsg>().Set(nus.msg, nus.fontSize, nus.colors, nus.endAction);
            }
        }
    }

    public void InsertNoticeQueue(string msg, float fontSize = 47, Color[] grd = null, Action endAction = null)
       => noticeQueue.Enqueue(new NoticeUISet(msg, fontSize, grd, endAction));

}