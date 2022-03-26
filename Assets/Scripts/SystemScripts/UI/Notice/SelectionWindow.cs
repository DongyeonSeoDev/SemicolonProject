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
                int si = i;  //���ϸ� AddListener���� ȣ���ϴ� �Լ����� ������ i ������ ��
                if (!activeWarning)
                {
                    btnList[si].onClick.AddListener(() => clickEv[si]());
                }
                else
                {
                    btnList[si].onClick.AddListener(() => UIManager.Instance.RequestWarningWindow(() => clickEv[si](), "������ Ȯ���մϱ�?"));
                }
                btnList[si].transform.GetChild(0).GetComponent<Text>().text = btnTexts[si];

                btnList[si].transform.SetParent(selBtnParent);  //Ʈ������ �� ������ ��ư���� �θ� ������
                btnList[si].transform.localScale = Vector3.one;  //������ ���� �ٸ� �� ������ �ʱ�ȭ������

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

    public void Inactive() //��Ȱ��ȭ�� �� �Լ��� ���ؼ�
    {
        transform.DOScale(SVector3.half, Global.fullScaleTransitionTime03).SetEase(Ease.OutCirc).SetUpdate(true);
        cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
    }

    public void Hide(bool hide)  //��� �� ���̰�
    {
        cvsg.alpha = hide?0:1;
        cvsg.blocksRaycasts = !hide;
        cvsg.interactable = !hide;
    }

    private void ActiveButtons(int count)  //�ʿ��� ������ŭ ��ư ������
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
        for (int i = 0; i < btnList.Count; i++)  //��ư Ȱ��ȭ ���ְ� ������ �θ�� ���� ����Ʈ �ʱ�ȭ
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
