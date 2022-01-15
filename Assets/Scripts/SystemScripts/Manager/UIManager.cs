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

        private void Update()
        {
            UserInput();
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
        }

        public void OnUIInteract(UIType type)
        {
            if (activeUIQueue.Count > 0) return;

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
            }
            else
            {
                activeUIList.Remove(ui);
            }
            activeUIQueue.Dequeue();
        }

   
    }
}