using DG.Tweening;
using UnityEngine;
using Water;

//나중에 전체적으로 코드 리펙토링 필요함
public class GameUI : MonoBehaviour
{
    private RectTransform rectTrm;
    private Vector3 originPos;

    [SerializeField] private CanvasGroup cvsg;
    public UIType _UItype;
    public GameUI childGameUI;
    public Pair<bool, Transform> setLastSibling; //UI가 켜지면 제일 아래로 정렬할지, 옮길 Transform

    [SerializeField] private bool isCloseReconfirmPair;
    [SerializeField] private string closeReconfirmMsg;
    public bool IsCloseable { get; set; }

    private GameUIFields gameUIFields;
    public GameUIFields UIFields { get { return gameUIFields; } }

    public Pair<bool, string> openUISoundPair;
    public Pair<bool, string> closeUISoundPair;


    public void ResetPos() => rectTrm.anchoredPosition = originPos;

    private void Awake()
    {
        rectTrm = GetComponent<RectTransform>();
        originPos = rectTrm.anchoredPosition;
        IsCloseable = false;
        gameUIFields = new GameUIFields() 
        { childGameUI = childGameUI, cvsg = cvsg, originPos = originPos, rectTrm = rectTrm, _UItype = _UItype, transform = transform, self = this };
    }


    public virtual void ActiveTransition()
    {
        gameObject.SetActive(true);
        UIManager.Instance.uiTweeningDic[_UItype] = true;
        if (openUISoundPair.first)
        {
           
            SoundManager.Instance.PlaySoundBox(openUISoundPair.second);
        }
        if(setLastSibling.first)
        {
            setLastSibling.second.SetAsLastSibling();
        }
        switch (_UItype)
        {
            case UIType.CHEF_FOODS_PANEL:
                //DOScale(true);
                TweeningData.DOScale(gameUIFields, true);
                break;

            case UIType.PRODUCTION_PANEL:
                //DOMove(true);
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.INVENTORY:
                //cvsg.alpha = 0f;
                //cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack());
                childGameUI.ActiveTransition();
                MenuPanel(true);
                break;

            case UIType.FOOD_DETAIL:
                //DOScale(true);
                TweeningData.DOScale(gameUIFields, true);
                break;

            case UIType.ITEM_DETAIL:
                //DOMove(true);
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.COMBINATION:
                cvsg.alpha = 0f;
                transform.localScale = SVector3.onePointSix;
                transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.InExpo).SetUpdate(true);
                cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack());
                break;

            case UIType.DEATH:
                //DOFade(true);
                TweeningData.DOFade(gameUIFields, true);
                break;

            case UIType.CLEAR:
                //DOFade(true);
                TweeningData.DOFade(gameUIFields, true);
                break;

           /* case UIType.KEYSETTING:
                //DOMoveSequence(true);
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;
            case UIType.RESOLUTION:
                //DOMoveSequence(true);
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.SOUND:
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.HELP:
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.CREDIT:
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;*/

            case UIType.SETTING:
                //DOFadeAndDissolve(true);
                //TweeningData.DOFadeAndDissolve(gameUIFields, true);
                MenuPanel(true);
                break;

            case UIType.STAT:
                MenuPanel(true);
                break;

            case UIType.MONSTER_COLLECTION:
                MenuPanel(true);
                break;

            case UIType.MONSTERINFO_DETAIL:
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.MONSTERINFO_DETAIL_STAT:
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.MONSTERINFO_DETAIL_ITEM:
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.MINIGAME_PICKUP:
                GameManager.Instance.pickupCheckGame.CheckStart();
                break;

            default:
                //DOScale(true);
                TweeningData.DOScale(gameUIFields, true);
                break;
        }
    }

    public virtual void InActiveTransition()
    {
        if(isCloseReconfirmPair && !IsCloseable)
        {
            IsCloseable = true;
            UIManager.Instance.CurrentReConfirmUI = this;
            UIManager.Instance.activeUIQueue.Dequeue();
            UIManager.Instance.SetReconfirmUI(closeReconfirmMsg, ()=> 
            {
                UIManager.Instance.activeUIQueue.Enqueue(false); 
                InActiveTransition();
                IsCloseable = false;
            }, () =>
            {
                IsCloseable = false;
            });
            return;
        }

        UIManager.Instance.uiTweeningDic[_UItype] = true;
        if (closeUISoundPair.first)
        {
            SoundManager.Instance.PlaySoundBox(closeUISoundPair.second);
        }
        switch (_UItype)
        {
            case UIType.CHEF_FOODS_PANEL:
                TweeningData.DOScale(gameUIFields, false);
                break;

            case UIType.PRODUCTION_PANEL:
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.INVENTORY:
                MenuPanel(false);
                childGameUI.InActiveTransition();
                break;

            case UIType.FOOD_DETAIL:
                TweeningData.DOScale(gameUIFields, false);
                break;

            case UIType.ITEM_DETAIL:
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.COMBINATION:
                transform.DOScale(SVector3.onePointSix, Global.fullScaleTransitionTime03).SetEase(Ease.OutQuad).SetUpdate(true);
                cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                break;

            case UIType.DEATH:
                TweeningData.DOFade(gameUIFields, false);
                break;

            case UIType.CLEAR:
                TweeningData.DOFade(gameUIFields, false);
                break;

            case UIType.SETTING:
                MenuPanel(false);
                break;

            case UIType.MONSTER_COLLECTION:
                MenuPanel(false);
                break;

            case UIType.MONSTERINFO_DETAIL:
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.STAT:
                MenuPanel(false);
                break;

            case UIType.MONSTERINFO_DETAIL_STAT:
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.MONSTERINFO_DETAIL_ITEM:
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.MINIGAME_PICKUP:
                GameManager.Instance.pickupCheckGame.Inactive();
                break;

            default:
                TweeningData.DOScale(gameUIFields, false);
                break;
        }
    }
    
    public virtual void ExceptionHandle()
    {

    }

    //리펙토링 필요
    private void OnDisable()
    {
        switch(_UItype)
        {
            case UIType.INVENTORY:
                UIManager.Instance.gameUIList[(int)UIType.ITEM_DETAIL].gameObject.SetActive(false);
                UIManager.Instance.selectedImg.gameObject.SetActive(false);
                break;
            case UIType.ITEM_DETAIL:
                UIManager.Instance.InActiveSpecialProcess(UIType.ITEM_DETAIL);
                break;
            case UIType.MONSTERINFO_DETAIL:
                UIManager.Instance.InActiveSpecialProcess(UIType.MONSTERINFO_DETAIL);
                break;
            case UIType.MONSTER_COLLECTION:
                UIManager.Instance.gameUIList[(int)UIType.MONSTERINFO_DETAIL].gameObject.SetActive(false);
                //UIManager.Instance.OnUIInteractSetActive(UIType.MONSTERINFO_DETAIL_FEATURE, false, true);
                //UIManager.Instance.OnUIInteractSetActive(UIType.MONSTERINFO_DETAIL_ITEM, false, true);
                //UIManager.Instance.OnUIInteractSetActive(UIType.MONSTERINFO_DETAIL_STAT, false, true);
                break;
        }
    }

    public void UpdateUIStack(bool add = true)
    {
        UIManager.Instance.UpdateUIStack(this, add);
    }

    public void MenuPanel(bool on)
    {
        if(on)
        {
            rectTrm.anchoredPosition = originPos - new Vector3(100,0);
            cvsg.alpha = 0.2f;

            rectTrm.DOAnchorPos(originPos, 0.3f).SetUpdate(true);
            cvsg.DOFade(1, 0.33f).SetUpdate(true).OnComplete(() => UpdateUIStack());
        }
        else
        {
            UpdateUIStack(false);
        }
    }
}