using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : MonoBehaviour
{
    [SerializeField]
    private List<TitleObject> menus = new List<TitleObject>();

    [SerializeField]
    private TitleMenuHighLighter menuTitleObjectHighlighter = null;

    [SerializeField]
    private int menuIdx = 0;
    private int maxMenuIdx = 0;

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
    }
    void Update()
    {
        CheckMenuIdx();
        DoMenuWork();
    }
    public void SetMenuIdx(int idx)
    {
        if(idx < 0)
        {
            menuIdx = 0;

            return;
        }
        else if(idx > maxMenuIdx)
        {
            menuIdx = maxMenuIdx;

            return;
        }

        menuIdx = idx;
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
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if(menuIdx <= 0)
            {
                menuIdx = 0;

                return;
            }

            menuIdx--;
        }

        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            if(menuIdx >= maxMenuIdx)
            {
                menuIdx = maxMenuIdx;

                return;
            }

            menuIdx++;
        }

        menuTitleObjectHighlighter.transform.position = menus[menuIdx].transform.position;
    }
}
