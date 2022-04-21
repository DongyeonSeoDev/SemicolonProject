using System.Collections;
using UnityEngine;

public class AcquisitionUI : MonoBehaviour
{
    public KeyAction keyType;
    [HideInInspector] public bool on = false;
    [SerializeField] private CanvasGroup cvsg;

    

    public void OnUIVisible(bool on)  //���߿� ���� �����ϰ� ���� �� �� �� ����� ���¿� ���� ó���� �ؾ���
    {
        this.on = on;
        cvsg.alpha = on ? 1 : 0;
        cvsg.interactable = on;
        cvsg.blocksRaycasts = on;
    }

    private void Start()
    {
        OnUIVisible(GameManager.Instance.savedData.userInfo.uiActiveDic[keyType]);
    }
}
