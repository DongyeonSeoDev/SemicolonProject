using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : MonoBehaviour
{
    private readonly string effectSoundBoxName = "UIMouseEnterSFX4";

    [SerializeField]
    private List<TitleObject> menus = new List<TitleObject>();

    [SerializeField]
    private TitleMenuHighLighter menuTitleObjectHighlighter = null;

    [SerializeField]
    private int menuIdx = 0;
    private int maxMenuIdx = 0;

    public bool canSetMenuIdx = true;

    void Start()
    {
        menus = GetComponentsInChildren<TitleObject>().ToList();

        if(menuTitleObjectHighlighter == null)
        {
            menuTitleObjectHighlighter = transform.parent.GetComponentInChildren<TitleMenuHighLighter>();
        }

        for(int i = 0; i < menus.Count; i++)
        {
            menus[i].curTitleObjIdx = i;
        }

        menuIdx = 0;
        maxMenuIdx = menus.Count - 1;
        menuTitleObjectHighlighter.transform.position = menus[0].transform.position;
    }
    void Update()
    {
        CheckMenuIdx();
        DoMenuWork();
    }
    public void SetMenuIdx(int idx)
    {
        if (!canSetMenuIdx || idx == menuIdx)
        {
            return;
        }

        if (idx < 0)
        {
            menuIdx = 0;

            return;
        }
        else if(idx > maxMenuIdx)
        {
            menuIdx = maxMenuIdx;

            return;
        }

        SoundManager.Instance.PlaySoundBox(effectSoundBoxName);
        menuIdx = idx;
        menuTitleObjectHighlighter.transform.position = menus[idx].transform.position;
    }
    private void DoMenuWork()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            menus[menuIdx].DoWork(); 
        }
    }
    private void CheckMenuIdx()
    {
        if(!canSetMenuIdx)
        {
            return;
        }

        int idx = menuIdx;

        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(idx <= 0)
            {
                idx = 0;

                return;
            }

            idx--;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(idx >= maxMenuIdx)
            {
                idx = maxMenuIdx;

                return;
            }

            idx++;
        }

        if (menuIdx != idx)
        {
            SetMenuIdx(idx);
        }
    }
}
