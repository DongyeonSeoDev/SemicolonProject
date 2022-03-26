using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Water;
using DG.Tweening;
using TMPro;

public class SelectionWindow : MonoBehaviour
{
    private List<Button> btnList = new List<Button>();
    private CanvasGroup cvsg;

    //public Text msgText;
    public TextMeshProUGUI msgTmp;
    public Transform selBtnParent;

    public void Set(string msg, List<Action> clickEv, List<string> btnTexts, bool activeWarning, List<Func<bool>> conditions)
    {
        //ResetData();
        ActiveButtons(clickEv.Count);

        msgTmp.text = msg;

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
                    btnList[si].onClick.AddListener(() => clickEv[si]());
                }
                else
                {
                    btnList[si].onClick.AddListener(() => UIManager.Instance.RequestWarningWindow(() => clickEv[si](), "결정이 확실합니까?"));
                }
                btnList[si].transform.GetChild(0).GetComponent<Text>().text = btnTexts[si];

                btnList[si].transform.SetParent(selBtnParent);  //트위닝이 다 끝나면 버튼들의 부모를 설정함
                btnList[si].transform.localScale = Vector3.one;  //스케일 값이 다를 수 있으니 초기화시켜줌

                if(conditions == null)
                {
                    btnList[si].interactable = true;
                    btnList[si].GetComponent<UIScale>().transitionEnable = true;
                }
                else
                {
                    if (conditions[si] == null)
                    {
                        btnList[si].interactable = true;
                        btnList[si].GetComponent<UIScale>().transitionEnable = true;
                    }
                    else
                    {
                        bool b = conditions[si]();
                        btnList[si].interactable = b;
                        btnList[si].GetComponent<UIScale>().transitionEnable = b;
                    }
                }
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
