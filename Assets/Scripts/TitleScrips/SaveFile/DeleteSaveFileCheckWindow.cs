using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DeleteSaveFileCheckWindow : TitlePopUpObject
{
    [SerializeField]
    private TitleDataController titleDataController = null;
    private SaveSlot currentSaveSlot;
    private SaveFileNum currentSaveFileNum;
    public void Init(SaveSlot saveSlot, SaveFileNum sfn)
    {
        currentSaveSlot = saveSlot;
        currentSaveFileNum = sfn;
    }

    public void DeleteSaveFile()
    {
        switch (currentSaveFileNum)
        {
            case SaveFileNum.num0:
                {
                    if (File.Exists(Global.SAVE_FILE_1.PersistentDataPath()))
                    {
                        File.Delete(Global.SAVE_FILE_1.PersistentDataPath());
                    }
                    else
                    {
                        return;
                    }
                }
                break;
            case SaveFileNum.num1:
                {
                    if (File.Exists(Global.SAVE_FILE_2.PersistentDataPath()))
                    {
                        File.Delete(Global.SAVE_FILE_2.PersistentDataPath());
                    }
                    else
                    {
                        return;
                    }
                }
                break;
            case SaveFileNum.num2:
                {
                    if (File.Exists(Global.SAVE_FILE_3.PersistentDataPath()))
                    {
                        File.Delete(Global.SAVE_FILE_3.PersistentDataPath());
                    }
                    else
                    {
                        return;
                    }
                }
                break;
        }

        titleDataController.loadedSlotNum--;
        currentSaveSlot.OnDelete();
        currentSaveSlot.Init();
    }
}
