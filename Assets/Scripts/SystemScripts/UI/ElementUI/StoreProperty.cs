using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreProperty : MonoBehaviour
{
    public Image statImg;
    public TextMeshProUGUI nameTMP;
    public Button btn;

    public ushort ID { get; private set; }

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {

        });
    }

    public void Renewal(ushort id)
    {
        ID = id;
        StatSO stat = NGlobal.playerStatUI.GetStatSOData(id);
        statImg.sprite = stat.statSpr;
        nameTMP.text = stat.statName;
    }
}
