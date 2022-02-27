using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Water;
using DG.Tweening;

public class SelectionWindow : MonoBehaviour
{
    private List<Button> btnList = new List<Button>();
    private CanvasGroup cvsg;

    public Text msgText;
    public Transform selBtnParent;

    public void Set(string msg, List<Action> clickEv, List<string> btnTexts, bool activeWarning)
    {
        //ResetData();
        ActiveButtons(clickEv.Count);

        msgText.text = msg;

        if (!cvsg) cvsg = GetComponent<CanvasGroup>();
        cvsg.alpha = 0;
        transform.localScale = SVector3.half;

        transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.OutCirc).SetUpdate(true);
        cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() =>
        {
            for (int i = 0; i < clickEv.Count; i++)
            {
                int si = i;  //안하면 AddListener에서 호출하는 함수에서 마지막 i 값으로 함
                if (!activeWarning)
                {
                    btnList[i].onClick.AddListener(() => clickEv[si]());
                }
                else
                {
                    UIManager.Instance.RequestWarningWindow(() => clickEv[si](), "결정이 확실합니까?");
                }
                btnList[i].transform.GetChild(0).GetComponent<Text>().text = btnTexts[i];

                btnList[i].transform.SetParent(selBtnParent);  //트위닝이 다 끝나면 버튼들의 부모를 설정함
                btnList[i].transform.localScale = Vector3.one;  //스케일 값이 다를 수 있으니 초기화시켜줌
            }
        });
    }

    public void Inactive() //비활성화는 이 함수를 통해서
    {
        transform.DOScale(SVector3.half, Global.fullScaleTransitionTime03).SetEase(Ease.OutCirc).SetUpdate(true);
        cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
    }

    public void Hide(bool hide)  //잠시 안 보이게
    {
        cvsg.alpha = hide?0:1;
        cvsg.blocksRaycasts = !hide;
        cvsg.interactable = !hide;
    }

    private void ActiveButtons(int count)  //필요한 개수만큼 버튼 가져옴
    {
        for(int i = 0; i<count; i++)
        {
            Button b = PoolManager.GetItem<Button>("SelBtn");
            b.onClick.RemoveAllListeners();
            btnList.Add(b);
        }
    }

    private void ResetData()
    {
        for (int i = 0; i < btnList.Count; i++)  //버튼 활성화 꺼주고 원래의 부모로 가고 리스트 초기화
        {
            btnList[i].gameObject.SetActive(false);
            btnList[i].transform.SetParent(transform.parent);
        }
        btnList.Clear();
    }

    private void OnDisable()
    {
        ResetData();
    }
}
