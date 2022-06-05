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

    [SerializeField] private Pair<float,float> checkRange; //성공을 체크하는 fillamount 범위 
    [SerializeField] private float plusRange; //fillamount가 몇 이상이면 1+1할지
    [SerializeField] private float speed; 
    [SerializeField] private float acceleration;
    private float currentSpeed;

    public VertexGradient successVG, failVG;

    private bool isReady = false; //미니게임 UI가 다 나왔는지
    private bool isGameStart = false; //미니 게임을 시작했는지
    public bool IsGameStart => isGameStart;
    public bool IsActivePickGame => isReady || isGameStart;

    private Vector3 tweeningMove = new Vector3(50, 0, 0);
    private WaitForSecondsRealtime wsr = new WaitForSecondsRealtime(0.4f);

    public Pick CurrentPick { get; private set; }

    public void InteractPick(Pick pick) //채집 시도 => 미니게임 시작
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
                yield return wsr;
                if (checkFill.fillAmount >= checkRange.first && checkFill.fillAmount <= checkRange.second) //체크 범위안에 들어옴 (성공)
                {
                    
                    EndGame(true, checkFill.fillAmount >= plusRange ? 2 : 1);
                }
                else  //체크 범위 밖에서 누름 (실패)
                {
                    EndGame(false);
                }
                
                break;
            }

            if(checkFill.fillAmount >= 1) //fillamount가 다 참 (실패)
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
        
    }

    public void Inactive()
    {
        TimeManager.TimeResume();
        EventManager.TriggerEvent("PickupMiniGame", false);
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
