using UnityEngine;
using UnityEngine.UI;

public class BuffSlot : MonoBehaviour
{
    public BuffStateDataSO buffData { get; private set; }
    public Image bufImg, coolTimeImg;
    public Text coolTimeTxt;
    public NameInfoFollowingCursor nifc;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.StateInfoDetail(buffData));
    }

    public void SetData(BuffStateDataSO data)
    {
        buffData = data;

        bufImg.sprite = data.sprite;
        //coolTimeImg.fillAmount = 0;
        coolTimeTxt.text = data.duration.ToString();

        nifc.explanation = buffData.stateName;
    }

    public void UpdateInfo()
    {
        coolTimeTxt.text = StateManager.Instance.stateCountDict[buffData.Id].ToString();
    }
}
