using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Water
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public List<GameUI> gameUIList = new List<GameUI>();
        private List<GameUI> activeUIList = new List<GameUI>();

        public Queue<bool> activeUIQueue = new Queue<bool>();

        public Image cursorInfoImg;
        public Text cursorInfoText;
        private RectTransform cursorImgRectTrm;
        private Vector3 cursorInfoImgOffset;

        private bool isOnCursorInfo = false;

        private void Awake()
        {
            cursorImgRectTrm = cursorInfoImg.GetComponent<RectTransform>();
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
            }
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

            //RectTransform rectTr_txt = cursorInfoText.GetComponent<RectTransform>();
            //cursorImgRectTrm.sizeDelta = new Vector2(rectTr_txt.rect.width + 100, rectTr_txt.rect.height + 100);
            cursorInfoImgOffset = new Vector3(cursorImgRectTrm.rect.width, -cursorImgRectTrm.rect.height) * 0.5f;

            cursorInfoImg.gameObject.SetActive(true);
        }

        public void OffCursorInfoUI()
        {
            cursorInfoImg.gameObject.SetActive(false);
            isOnCursorInfo = false;
        }
    }
}