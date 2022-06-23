using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    public void Init()
    {
        SaveFileStream.LoadGameSaveData(saveFileName);
    }

    public void OnStart()
    {
        SaveFileStream.currentSaveFileName = saveFileName;
    }

    public void OnDelete()
    {
        SaveFileStream.DeleteGameSaveData(saveFileName);
    }
}
