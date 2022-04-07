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
    public Transform selBtnParent, iconSelBtnParent;

    /// <summary>
    /// 메시지, 순서대로 클릭했을 때의 반응, 순서대로 클릭 버튼에 띄울 텍스트, 경고창 띄울지, 각 버튼마다 눌리게 할 조건, 아이콘으로 표시할지
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="clickEv"></param>
    /// <param name="btnTexts"></param>
    /// <param name="activeWarning"></param>
    /// <param name="conditions"></param>
    public void Set(string msg, List<Action> clickEv, List<string> btnTexts, bool activeWarning, List<Func<bool>> conditions, bool useIcon)
    {
        //ResetData();
        Transform btnPar = !useIcon ? selBtnParent : iconSelBtnParent;

        ActiveButtons(clickEv.Count, useIcon);

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

                btnList[si].transform.SetParent(btnPar);  //트위닝이 다 끝나면 버튼들의 부모를 설정함
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

                if(!useIcon)
                {
                    btnList[si].transform.GetChild(0).GetComponent<Text>().text = btnTexts[si];
                }
                else
                {
                    Triple<Sprite, string, string> data = UIManager.Instance.iconSelBtnDataDic[btnTexts[si]];

                    btnList[si].GetComponent<Image>().sprite = data.first;
                    btnList[si].transform.GetChild(0).GetComponent<Text>().text = data.second;
                    btnList[si].GetComponent<NameInfoFollowingCursor>().explanation = data.third;
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

    private void ActiveButtons(int count, bool useIcon)  //필요한 개수만큼 버튼 가져옴
    {
        string key = !useIcon ? "SelBtn" : "IconSelBtn";

        for (int i = 0; i<count; i++)
        {
            Button b = PoolManager.GetItem<Button>(key);
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
