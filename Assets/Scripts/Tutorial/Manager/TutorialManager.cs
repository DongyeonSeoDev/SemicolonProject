using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager> 
{
    private GameManager gm;
    private UIManager um;

    public bool IsTutorialStage { get; set; }

    private void Awake()
    {
        
    }

    private void Start()
    {
        gm = GameManager.Instance;
        um = UIManager.Instance;

        if(!gm.savedData.tutorialInfo.isEnded)
        {
            IsTutorialStage = true;

            
        }
    }
}
