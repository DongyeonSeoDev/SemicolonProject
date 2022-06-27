using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private string saveFileName;

    public Button continueBtn;

    // ���⿡ �ؽ�Ʈ����� �׷��� ���� �Ű������鵵 ����д�.

    public bool IsEmptySlot => !SaveFileStream.HasSaveData(saveFileName);

    public void Init()
    {
        SaveFileStream.LoadGameSaveData(saveFileName);

        continueBtn.onClick.AddListener(() =>
        {
            OnStart();
            LoadSceneManager.Instance.LoadScene("StageScene");  //�ӽ� �ڵ�
        });

        //�������� ���� UI �����. (���� ����)
    }

    public void OnStart()
    {
        SaveFileStream.currentSaveFileName = saveFileName;
    }

    public void OnDelete()
    {
        SaveFileStream.DeleteGameSaveData(saveFileName);

        //UI ����
    }
}
