using UnityEngine;

public class TitleDataController : MonoBehaviour
{
    public readonly int maxSlotNum = 3;

    public SaveSlot[] saveSlots;
    public int loadedSlotNum = 0;

    private void Awake()
    {
        Time.timeScale = 1;
        SaveFileStream.LoadOption();
        for(int i = 0; i < saveSlots.Length; i++)
        {
            if(saveSlots[i].Init())
            {
                loadedSlotNum++;
            }
        }

        EventManager.StartListening("StartNewGame", StartNewGame);
        EventManager.StartListening("StartStageScene", () =>
        {
            Save();
            StoredData.Reset();
        });
    }

    public void Save()
    {
        SaveFileStream.SaveOption();
    }

    public void StartNewGame()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if(saveSlots[i].IsEmptySlot)
            {
                saveSlots[i].OnStart();
                return;
            }
        }

        Debug.Log("ºó ½½·ÔÀÌ ¾øÀ½");
        //ºó ½½·Ô ¾øÀ» ¶§ÀÇ Ã³¸®
    }

    #region OnApplication
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            Save();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }

    #endregion
}
