using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Water
{
    public class UIManager : MonoSingleton<UIManager>
    {
        #region ���� UI ���� ������
        public List<GameUI> gameUIList = new List<GameUI>();
        private List<GameUI> activeUIList = new List<GameUI>();

        public Queue<bool> activeUIQueue = new Queue<bool>(); //� UI�� �����ų� ������ �ִϸ��̼�(Ʈ����) ���� �߿� �ٸ� UI (��)Ȱ��ȭ ���� ���� ����
        #endregion

        #region ���콺 ����ٴϴ� ���� UI���� ������
        [Space(20)]
        public Image cursorInfoImg;  //���콺 ����ٴϴ� ���� �ؽ�Ʈ�� �ִ� �̹���
        public Text cursorInfoText; // ���콺 ����ٴϴ� ���� �ؽ�Ʈ
        private RectTransform cursorImgRectTrm;
        private Vector3 cursorInfoImgOffset;

        private bool isOnCursorInfo = false;  //���콺 ����ٴϴ� ���� �ؽ�Ʈ Ȱ��ȭ �����ΰ�
        private float sw; //cursorImgRectTrm�� ó�� �ʺ�(�ּ� �ʺ�)
        public float widthOffset = 39;  //���콺 ����ٴϴ� ���� �ؽ�Ʈ���� �̹��� �ʺ� Ű�ﶧ ���ڴ� Ű�� ����
        #endregion

        #region Inventory Item Detail View
        private int selectedItemId = -1;

        [Space(20)]
        public Image itemImg, itemTypeImg;
        public Text itemNameTxt, itemExplanation;
        public Text itemCntTxt, itemTypeTxt;
        public Button itemUseBtn, itemJunkBtn;
        #endregion

        [Space(20)]
        public GameObject systemMsgPrefab;
        public Transform systemMsgParent;

        private GameManager gm;

        private void Awake()
        {
            cursorImgRectTrm = cursorInfoImg.GetComponent<RectTransform>();
            sw = cursorImgRectTrm.rect.width;
        }

        private void Start()
        {
            gm = GameManager.Instance;
            PoolManager.CreatePool(systemMsgPrefab, systemMsgParent, 5, "SystemMsg");
        }

        private void Update()
        {
            UserInput();
            CursorInfo();
        }

        private void UserInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(activeUIList.Count>0)
                {
                    OnUIInteract(activeUIList[activeUIList.Count - 1]._UItype);
                }
            }
            else if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.INVENTORY]))
            {
                OnUIInteract(UIType.INVENTORY);
            }
        }

        public void OnUIInteractBtnClick(int type) { OnUIInteract((UIType)type); }

        public void OnUIInteract(UIType type, bool ignoreQueue = false)
        {
            if (activeUIQueue.Count > 0 && !ignoreQueue) return;

            activeUIQueue.Enqueue(false);
            GameUI ui = gameUIList[(int)type];

            if(!ui.gameObject.activeSelf)
            {
                ui.gameObject.SetActive(true);
            }
            else
            {
                ui.InActiveTransition();
            }
        }

        public void UpdateUIStack(GameUI ui , bool add = true)
        {
            if(add)
            {
                activeUIList.Add(ui);
                ActiveSpecialProcess(ui._UItype);
            }
            else
            {
                activeUIList.Remove(ui);
                ui.gameObject.SetActive(false);
                InActiveSpecialProcess(ui._UItype);
            }
            activeUIQueue.Dequeue();
        }

        private void ActiveSpecialProcess(UIType type)
        {
            switch(type)
            {

            }
        }

        private void InActiveSpecialProcess(UIType type)
        {
            switch (type)
            {
                case UIType.PRODUCTION_PANEL:
                    CookingManager.Instance.MakeFoodInfoUIReset();
                    break;
                case UIType.FOOD_DETAIL:
                    CookingManager.Instance.detailID = -1;
                    break;
                case UIType.ITEM_DETAIL:
                    selectedItemId = -1;
                    break;
            }
        }

        public void UIPositionReset(UIType type)
        {
            gameUIList[(int)type].ResetPos();
        }

        public void AllUIPositionReset()
        {
            gameUIList.ForEach(ui => ui.ResetPos());
        }

        private void CursorInfo()
        {
            if(isOnCursorInfo)
            {
                cursorImgRectTrm.position = Input.mousePosition + cursorInfoImgOffset;
            }
        }

        public void SetCursorInfoUI(string msg, int fontSize = 39)
        {
            isOnCursorInfo = true;

            cursorInfoText.text = msg;
            cursorInfoText.fontSize = fontSize;

            cursorImgRectTrm.sizeDelta = new Vector2(Mathf.Clamp(msg.Length*widthOffset,sw,1000f), cursorImgRectTrm.rect.height);
            cursorInfoImgOffset = new Vector3(cursorImgRectTrm.rect.width, -cursorImgRectTrm.rect.height) * 0.5f;

            cursorInfoImg.gameObject.SetActive(true);
        }

        public void OffCursorInfoUI()
        {
            cursorInfoImg.gameObject.SetActive(false);
            isOnCursorInfo = false;
        }

        public void DetailItemSlot(int itemID)  //�κ��丮���� ������ ���� Ŭ��
        {
            if (selectedItemId == itemID) return;
            else if(selectedItemId == -1) OnUIInteract(UIType.ITEM_DETAIL);
            selectedItemId = itemID;

            ItemSO data = gm.GetItemData(itemID);

            itemImg.sprite = data.GetSprite();
            itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
            itemNameTxt.text = data.itemName;
            itemExplanation.text = data.explanation;
            itemCntTxt.text = string.Format("����: {0}��", gm.GetItemCount(itemID));
            itemTypeTxt.text = Global.GetItemTypeName(data.itemType);

            itemUseBtn.gameObject.SetActive(data.itemType!=ItemType.ETC);
            itemUseBtn.onClick.AddListener(() => data.Use());
        }

        public void RequestSystemMsg(string msg, int fontSize = 35, float existTime = 1.5f)
        {
            PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime);
        }
        

        
    }
}