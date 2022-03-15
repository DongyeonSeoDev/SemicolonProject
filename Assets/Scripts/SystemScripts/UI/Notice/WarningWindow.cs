using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class WarningWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI warningTmp;

    [SerializeField] private Button confirmBtn;

    [SerializeField] private Pair<Text, Text> cancelAndVerify;

    [SerializeField] CanvasGroup cvsg;

    private void ExecuteTweening()
    {
        transform.DOKill();
        cvsg.DOKill();

        cvsg.alpha = 0;
        //cvsg.interactable = false;
        //cvsg.blocksRaycasts = false;
        transform.localScale = SVector3.zeroPointSeven;

        transform.DOScale(Vector3.one, 0.3f).SetUpdate(true);
        cvsg.DOFade(1, 0.5f).SetUpdate(true);
    }

    public void Register(System.Action confirmAc, string warning, string confirmTx, string cancelTx)
    {
        gameObject.SetActive(true);
        ExecuteTweening();

        confirmBtn.onClick.RemoveAllListeners();
        confirmAc += DefaultConfirmAction;
        confirmBtn.onClick.AddListener(()=>confirmAc());

        warningTmp.SetText(warning);
        cancelAndVerify.first.text = cancelTx;
        cancelAndVerify.second.text = confirmTx;
    }

    private void DefaultConfirmAction()
    {
        Cancel();
    }

    public void Cancel()
    {
        transform.DOKill();
        cvsg.DOKill();

        transform.DOScale(SVector3.zeroPointSeven, 0.3f).SetUpdate(true);
        cvsg.DOFade(0, 0.3f).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
    }
}
