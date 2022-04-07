using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using TMPro;

public class PickupCheck : MonoBehaviour
{
    private GameUI pickMiniGameUI;
    public CanvasGroup pickupCircleCheckGamePanel, circleCvsg;
    private RectTransform circleRectTr;
    private Vector3 originPos;

    public Text startTxt;

    [SerializeField] private Image checkFill;

    [SerializeField] private Pair<float,float> checkRange;
    [SerializeField] private float plusRange;
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    private float currentSpeed;

    public VertexGradient successVG, failVG;

    private bool isReady = false;
    private bool isGameStart = false;
    public bool IsGameStart => isGameStart;

    private Vector3 tweeningMove = new Vector3(50, 0, 0);
    private WaitForSecondsRealtime wsr = new WaitForSecondsRealtime(1f);

    public Pick CurrentPick { get; private set; }

    public void InteractPick(Pick pick)
    {
        CurrentPick = pick;
        UIManager.Instance.OnUIInteractSetActive(UIType.MINIGAME_PICKUP, true, true);
        TimeManager.TimePause();
    }

    public void CheckStart()
    {
        EventManager.TriggerEvent("PickupMiniGame", true);

        if(!circleRectTr)
        {
            circleRectTr = circleCvsg.GetComponent<RectTransform>();
            originPos = circleRectTr.anchoredPosition;
            pickMiniGameUI = pickupCircleCheckGamePanel.GetComponent<GameUI>();
        }

        pickupCircleCheckGamePanel.alpha = 0f;
        circleCvsg.alpha = 0f;
        circleRectTr.anchoredPosition = originPos - tweeningMove;
        checkFill.fillAmount = 0;

        startTxt.gameObject.SetActive(true);
        startTxt.text = "시작하기 : " + KeyCodeToString.GetString(KeySetting.keyDict[KeyAction.EVENT]);
        currentSpeed = speed;
        
        //플레이어 조작 막아야 함

        pickupCircleCheckGamePanel.DOFade(1, 0.3f).SetUpdate(true);
        circleRectTr.DOAnchorPos(originPos, 0.35f).SetUpdate(true);
        circleCvsg.DOFade(1, 0.4f).SetUpdate(true).OnComplete(() =>
        {
            isReady = true;
            UIManager.Instance.UpdateUIStack(pickMiniGameUI);
        });
    }

    private void Update()
    {
        if(isReady)
        {
            if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.EVENT]))
            {
                isReady = false;
                isGameStart = true;
                startTxt.gameObject.SetActive(false);
                StartCoroutine(GameUpdateCo());
            }
        }
    }

    private IEnumerator GameUpdateCo()
    {
        //yield return wsr;

        while(isGameStart)
        {
            yield return null;

            checkFill.fillAmount += currentSpeed * Time.unscaledDeltaTime;
            currentSpeed += acceleration * Time.unscaledDeltaTime;

            if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.EVENT]))
            {
                if (checkFill.fillAmount >= checkRange.first && checkFill.fillAmount <= checkRange.second)
                {
                    
                    EndGame(true, checkFill.fillAmount >= plusRange ? 2 : 1);
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
                yield return wsr;
                EndGame(false);
            }
        }
    }

    private void EndGame(bool success, int count = 0)
    {
        //UIManager.Instance.InsertTopCenterNoticeQueue(success ? "성공" :"실패",success?successVG:failVG ,82, null, 1);

        pickMiniGameUI.IsCloseable = true;
        isGameStart = false;
        UIManager.Instance.OnUIInteractSetActive(UIType.MINIGAME_PICKUP, false, true);
        CurrentPick.PickResult(success, count);
        EventManager.TriggerEvent("PickupMiniGame", false);
    }

    public void Inactive()
    {
        TimeManager.TimeResume();
        isGameStart = false;
        circleCvsg.DOFade(0, 0.2f).SetUpdate(true);
        pickupCircleCheckGamePanel.DOFade(0, 0.25f).SetUpdate(true).OnComplete(() =>
        {
            //플레이어 조작 다시 되게 해야 함
            pickupCircleCheckGamePanel.gameObject.SetActive(false);
            UIManager.Instance.UpdateUIStack(pickupCircleCheckGamePanel.GetComponent<GameUI>(), false);
            pickMiniGameUI.IsCloseable = false;
        });
    }
}
