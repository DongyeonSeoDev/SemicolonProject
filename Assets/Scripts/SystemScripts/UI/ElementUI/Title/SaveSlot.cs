using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    public Button continueBtn;

    public bool IsEmptySlot => !SaveFileStream.HasSaveData(saveFileName);

    public void Init()
    {
        SaveFileStream.LoadGameSaveData(saveFileName);

        continueBtn.onClick.AddListener(() =>
        {
            OnStart();
            LoadSceneManager.Instance.LoadScene("StageScene");  //임시 코드
        });

        //여러가지 정보 UI 띄워줌. (저장 정보)
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
