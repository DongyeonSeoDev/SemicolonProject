using UnityEngine;

public class TitleDataController : MonoBehaviour
{
    public SaveSlot[] saveSlots;

    private void Awake()
    {
        Time.timeScale = 1;
        SaveFileStream.LoadOption();
        for(int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].Init();
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
                break;
            }
        }

        Debug.Log("�� ������ ����");
        //�� ���� ���� ���� ó��
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
