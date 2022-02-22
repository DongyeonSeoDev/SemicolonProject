using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Water;

public class SelectionWindow : MonoBehaviour
{
    private List<Button> btnList = new List<Button>();

    public Text msgText;
    public Transform selBtnParent;

    public void Set(string msg, List<Action> clickEv, List<string> btnTexts)
    {
        msgText.text = msg;
        ResetData();
        ActiveButtons(clickEv.Count);

        for(int i=0; i<clickEv.Count; i++)
        {
            int si = i;
            btnList[i].onClick.AddListener(() => clickEv[si]());
            btnList[i].transform.GetChild(0).GetComponent<Text>().text = btnTexts[i];
        }
    }

    private void ResetData()
    {
        for(int i=0; i<btnList.Count; i++)
        {
            btnList[i].onClick.RemoveAllListeners();
            btnList[i].gameObject.SetActive(false);
        }
    }

    private void ActiveButtons(int count)
    {
        int idx = 0, i;
        for (i = 0; i < btnList.Count; i++)
        {
            idx++;
            btnList[idx].gameObject.SetActive(true);
        }
        for(i = idx; i<count; i++)
        {
            Button b = PoolManager.GetItem<Button>("SelBtn");
            b.transform.parent = selBtnParent;
            btnList.Add(b);
        }
    }
}
