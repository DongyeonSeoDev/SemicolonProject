using System.Collections;
using UnityEngine;

public class AcquisitionUI : MonoBehaviour
{
    public UIType uiType;
    [HideInInspector] public bool on = false;
    private CanvasGroup cvsg;

    private void Awake()
    {
        cvsg = GetComponent<CanvasGroup>();
        
    }

    public void OnUIVisible(bool on)
    {
        this.on = on;
        cvsg.alpha = on ? 1 : 0;
        cvsg.interactable = on;
        cvsg.blocksRaycasts = on;
    }

    private IEnumerator Start()
    {
        while (!StoredData.HasObjectKey("SetUIAcqState")) yield return null;

        OnUIVisible(GameManager.Instance.savedData.userInfo.uiActiveDic[uiType]);

        if (TutorialManager.Instance.IsTestMode && !on)  //Test
        {
            OnUIVisible(true);
            GameManager.Instance.savedData.userInfo.uiActiveDic[uiType] = true;
        }
    }
}
