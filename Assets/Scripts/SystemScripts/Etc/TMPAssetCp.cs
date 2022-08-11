using UnityEngine;
using TMPro;

public class TMPAssetCp : MonoBehaviour
{
    protected Material newMat;

    private void Awake()
    {
        CopyFontAsset();
    }

    public void CopyFontAsset()
    {
        /*TMP_FontAsset newFont = Instantiate(GetComponent<TextMeshProUGUI>().font);
        GetComponent<TextMeshProUGUI>().font = newFont;
        Material newMat = Instantiate(newFont.material);
        newFont.material = newMat;*/

        newMat = Instantiate(GetComponent<TextMeshProUGUI>().fontMaterial);
        GetComponent<TextMeshProUGUI>().fontMaterial = newMat;
    }
}
