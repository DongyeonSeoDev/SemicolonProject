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
                int si = i;  //���ϸ� AddListener���� ȣ���ϴ� �Լ����� ������ i ������ ��
                if (!activeWarning)
                {
                    btnList[i].onClick.AddListener(() => clickEv[si]());
                }
                else
                {
                    UIManager.Instance.RequestWarningWindow(() => clickEv[si](), "������ Ȯ���մϱ�?");
                }
                btnList[i].transform.GetChild(0).GetComponent<Text>().text = btnTexts[i];

                btnList[i].transform.SetParent(selBtnParent);  //Ʈ������ �� ������ ��ư���� �θ� ������
                btnList[i].transform.localScale = Vector3.one;  //������ ���� �ٸ� �� ������ �ʱ�ȭ������
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
