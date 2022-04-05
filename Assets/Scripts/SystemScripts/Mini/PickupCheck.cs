using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class PickupCheck : MonoBehaviour
{
    public CanvasGroup pickupCircleCheckGamePanel, circleCvsg;
    private RectTransform circleRectTr;
    private Vector3 originPos;

    [SerializeField] private Image checkFill;

    [SerializeField] private Pair<float,float> checkRange;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;

    public VertexGradient successVG, failVG;

    private bool isGameStart = false;

    private Vector3 tweeningMove = new Vector3(50, 0, 0);
    private WaitForSecondsRealtime wsr = new WaitForSecondsRealtime(2f);

    public Pick CurrentPick { get; private set; }

    public void CheckStart(Pick pick)
    {
        if(!circleRectTr)
        {
            circleRectTr = circleCvsg.GetComponent<RectTransform>();
            originPos = circleRectTr.anchoredPosition;
        }

        pickupCircleCheckGamePanel.alpha = 0f;
        circleCvsg.alpha = 0f;
        circleRectTr.anchoredPosition = originPos - tweeningMove;
        checkFill.fillAmount = 0;
        pickupCircleCheckGamePanel.gameObject.SetActive(true);
        
        //플레이어 조작 막아야 함

        pickupCircleCheckGamePanel.DOFade(1, 0.3f).SetUpdate(true);
        circleRectTr.DOAnchorPos(originPos, 0.35f).SetUpdate(true);
        circleCvsg.DOFade(1, 0.4f).SetUpdate(true).OnComplete(() =>
        {
            UIManager.Instance.InsertTopCenterNoticeQueue("시작", 90, () =>
            {
                isGameStart = true;
                StartCoroutine(GameUpdateCo());
            }, 1f);
        });

        CurrentPick = pick;
    }

    private IEnumerator GameUpdateCo()
    {
        yield return wsr;

        while(isGameStart)
        {
            yield return null;

            checkFill.fillAmount += speed * Time.unscaledDeltaTime;
            speed += acceleration * Time.unscaledDeltaTime;

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.INTERACTION]))
            {
                if (checkFill.fillAmount >= checkRange.first && checkFill.fillAmount <= checkRange.second)
                {
                    EndGame(true);
                }
                else
                {
                    EndGame(false);
                }
                yield return wsr;
                break;
            }
            if(checkFill.fillAmount >= 1)
            {
                EndGame(false);
            }
        }
    }

    private void EndGame(bool success)
    {
        isGameStart = false;

        UIManager.Instance.InsertTopCenterNoticeQueue(success ? "성공" :"실패",success?successVG:failVG ,82, null, 1);
        UIManager.Instance.Notice();
        circleCvsg.DOFade(0, 0.3f).SetUpdate(true);
        pickupCircleCheckGamePanel.DOFade(0, 0.33f).SetUpdate(true).OnComplete(() =>
        {
            //플레이어 조작 다시 되게 해야 함
            pickupCircleCheckGamePanel.gameObject.SetActive(false);
            CurrentPick.PickResult(success);
        });
    }
}
