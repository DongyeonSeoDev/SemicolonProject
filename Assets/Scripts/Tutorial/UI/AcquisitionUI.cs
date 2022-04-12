
using UnityEngine;

public class AcquisitionUI : MonoBehaviour
{
    public UIType uiType;
    [HideInInspector] public bool on = false;
    private CanvasGroup cvsg;

    private void Awake()
    {
        cvsg = GetComponent<CanvasGroup>();
        //OnUIVisible(false);
    }

    public void OnUIVisible(bool on)
    {
        this.on = on;
        cvsg.alpha = on ? 1 : 0;
        cvsg.interactable = on;
        cvsg.blocksRaycasts = on;
    }

    private void Start()
    {
        UserInfo info = GameManager.Instance.savedData.userInfo;
        bool active = info.uiActiveDic.keyValueDic.ContainsKey(uiType) && info.uiActiveDic[uiType];
        OnUIVisible(active);
        if(!active)
        {
            UIActiveData.Instance.uiActiveDic[uiType] = false;
        }
        if(TutorialManager.Instance.IsTestMode && !on)  //Test
        {
            OnUIVisible(true);
        }
    }
}
