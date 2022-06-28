using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum SaveFileNum
{
    num0,
    num1,
    num2
}
public class DeleteSaveFileData : EnableTitlePopUpObject
{
    [SerializeField]
    private DeleteSaveFileCheckWindow window;
    private SaveSlot saveSlot = null;

    [SerializeField]
    private SaveFileNum saveFileNum;

    public void Start()
    {
        //base.Start();

        saveSlot = transform.parent.GetComponent<SaveSlot>();
    }

    public override void DoWork()
    {
        base.DoWork();

        window.gameObject.SetActive(true);
        window.Init(saveSlot, saveFileNum);
    }
}
