using System.Collections;
using UnityEngine;

public class AcquisitionUI : MonoBehaviour
{
    public KeyAction keyType;
    [HideInInspector] public bool on = false;
    [SerializeField] private CanvasGroup cvsg;

    

    public void OnUIVisible(bool on)  //나중에 게임 시작하고 나서 한 번 더 저장된 상태에 따라서 처리를 해야함
    {
        this.on = on;
        cvsg.alpha = on ? 1 : 0;
        cvsg.interactable = on;
        cvsg.blocksRaycasts = on;
    }

    /*private IEnumerator Start()
    {
        while (!StoredData.HasObjectKey("SetUIAcqState")) yield return null;

        OnUIVisible(GameManager.Instance.savedData.userInfo.uiActiveDic[uiType]);

        if (TutorialManager.Instance.IsTestMode && !on)  //Test
        {
            OnUIVisible(true);
            GameManager.Instance.savedData.userInfo.uiActiveDic[uiType] = true;
        }
    }*/
}
